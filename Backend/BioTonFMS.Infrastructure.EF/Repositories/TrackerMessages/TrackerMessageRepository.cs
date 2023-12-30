using Azure;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Paging.Extensions;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;

public class TrackerMessageRepository : Repository<TrackerMessage, MessagesDBContext>, ITrackerMessageRepository
{
    private readonly ITrackerTagRepository _tagsRepository;
    private readonly ITrackerRepository _trackerRepository;

    public TrackerMessageRepository(IKeyValueProvider<TrackerMessage, int> keyValueProvider,
        IQueryableProvider<TrackerMessage> queryableProvider,
        UnitOfWorkFactory<MessagesDBContext> unitOfWorkFactory,
        ITrackerTagRepository tagsRepository,
        ITrackerRepository trackerRepository)
        : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
        _tagsRepository = tagsRepository;
        _trackerRepository = trackerRepository;
    }

    public TrackerMessage? this[long key] => HydratedQuery.FirstOrDefault(x => x.Id == key);

    private IQueryable<TrackerMessage> HydratedQuery => QueryableProvider.Fetch(m => m.Tags).Linq();

    public override void Add(TrackerMessage entity)
    {
        entity.ServerDateTime = SystemTime.UtcNow;
        base.Add(entity);
    }

    public IList<TrackerMessage> GetMessages(bool forUpdate)
    {
        var query = forUpdate ? HydratedQuery : HydratedQuery.AsNoTracking();
        query = query
            .OrderBy(m => m.ExternalTrackerId)
            .ThenBy(m => m.Id);
        return query.ToList();
    }

    public IList<TrackerMessage> GetMessagesForTrackers(int[] trackerExternalIds, bool forUpdate = false)
    {
        var query = forUpdate ? HydratedQuery : HydratedQuery.AsNoTracking();
        query = query
            .Where(m => trackerExternalIds.Contains(m.ExternalTrackerId))
            .OrderBy(m => m.ExternalTrackerId)
            .ThenBy(m => m.Id);
        return query.ToList();
    }

    public IList<TrackerMessage> GetTrackerMessagesForDate(int[] trackerExternalIds, DateOnly date,
        bool forUpdate = false)
    {
        var query = forUpdate ? HydratedQuery : HydratedQuery.AsNoTracking();
        query = query
            .Where(m => trackerExternalIds.Contains(m.ExternalTrackerId) &&
                        DateOnly.FromDateTime(m.ServerDateTime) == date)
            .OrderBy(m => m.ExternalTrackerId)
            .ThenBy(m => m.Id);
        return query.ToList();
    }

    public bool ExistsByUid(Guid uid) =>
        QueryableProvider.Linq().AsNoTracking().Any(x => x.PackageUID == uid);

    public TrackerStandardParameters GetStandardParameters(int externalId)
    {
        var stdParams = new TrackerStandardParameters();

        var last = QueryableProvider.Linq().AsNoTracking()
            .Where(x => x.ExternalTrackerId == externalId)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault();

        if (last is null) return stdParams;

        stdParams.Time = last.ServerDateTime;

        stdParams.Long = last.Longitude ?? QueryableProvider.Linq()
            .Where(x => x.Longitude != null && x.ExternalTrackerId == externalId)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Longitude;

        stdParams.Lat = last.Latitude ?? QueryableProvider.Linq()
            .Where(x => x.Latitude != null && x.ExternalTrackerId == externalId)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Latitude;

        stdParams.Speed = last.Speed ?? QueryableProvider.Linq()
            .Where(x => x.Speed != null && x.ExternalTrackerId == externalId)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Speed;

        stdParams.Alt = last.Altitude ?? QueryableProvider.Linq()
            .Where(x => x.Altitude != null && x.ExternalTrackerId == externalId)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Altitude;

        return stdParams;
    }

    public IList<TrackerParameter> GetLastParameters(int externalId)
    {
        if (externalId == 0)
        {
            return new List<TrackerParameter>();
        }

        IList<MessageTag>? lastTags = QueryableProvider
            .Fetch(m => m.Tags.Where(x => x.SensorId == null))
            .Linq()
            .AsNoTracking()
            .Where(x => x.ExternalTrackerId == externalId)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Tags;

        if (lastTags is null || !lastTags.Any())
        {
            return new List<TrackerParameter>();
        }

        Dictionary<int, TrackerTag> trackerTags = _tagsRepository.GetTags().ToDictionary(x => x.Id);
        var result = new List<TrackerParameter>();
        foreach (var tag in lastTags)
        {
            var parameter = new TrackerParameter();
            if (tag.TrackerTagId.HasValue)
            {
                parameter.ParamName = trackerTags[tag.TrackerTagId.Value].Name;
            }

            switch (tag.TagType)
            {
                case TagDataTypeEnum.Integer:
                    parameter.LastValueDecimal = ((MessageTagInteger)tag).Value;
                    break;
                case TagDataTypeEnum.Bits or TagDataTypeEnum.Boolean or TagDataTypeEnum.String:
                    parameter.LastValueString = tag.ValueString;
                    break;
                case TagDataTypeEnum.Byte:
                    parameter.LastValueDecimal = ((MessageTagByte)tag).Value;
                    break;
                case TagDataTypeEnum.Double:
                    parameter.LastValueDecimal = ((MessageTagDouble)tag).Value;
                    break;
                case TagDataTypeEnum.DateTime:
                    parameter.LastValueDateTime = ((MessageTagDateTime)tag).Value;
                    break;
            }

            result.Add(parameter);
        }

        return result;
    }

    public TrackerMessage? GetLastMessageFor(int externalTrackerId)
    {
        if (externalTrackerId == 0)
            return null;

        return HydratedQuery
            .AsNoTracking()
            .Where(m => m.ExternalTrackerId == externalTrackerId)
            .OrderByDescending(m => m.Id)
            .FirstOrDefault();
    }

    public PagedResult<ParametersHistoryRecord> GetParametersHistory(ParametersHistoryFilter filter)
    {
        Dictionary<int, string> tagNames = _tagsRepository.GetTags()
            .ToDictionary(x => x.Id, x => x.Name);

        var page = QueryableProvider
            .Fetch(m => m.Tags.Where(x => x.SensorId == null))
            .Linq()
            .AsNoTracking()
            .Where(x => x.ExternalTrackerId == filter.ExternalId)
            .OrderByDescending(x => x.ServerDateTime)
            .GetPagedQueryable(filter.PageNum, filter.PageSize);

        return new PagedResult<ParametersHistoryRecord>
        {
            PageSize = page.PageSize,
            CurrentPage = page.CurrentPage,
            TotalRowCount = page.TotalRowCount,
            TotalPageCount = page.TotalPageCount,
            Results = page.Results.Select(x => new ParametersHistoryRecord
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Altitude = x.Altitude,
                Speed = x.Speed,
                Time = x.ServerDateTime,
                Parameters = x.Tags.Aggregate("", (res, tag) =>
                    res + tagNames[tag.TrackerTagId!.Value] + '=' + tag.ValueString + ',')
            }).ToList()
        };
    }

    public IDictionary<int, VehicleStatus> GetVehicleStates(int[] externalIds,
        int trackerAddressValidMinutes)
    {
        var now = SystemTime.UtcNow;

        var foundResults = QueryableProvider.Linq()
            .Where(x => externalIds.Contains(x.ExternalTrackerId))
            .GroupBy(x => x.ExternalTrackerId,
                (key, g) => g
                    .OrderByDescending(x => x.ServerDateTime)
                    .Select(x => new VehicleStatus
                    {
                        TrackerExternalId = x.ExternalTrackerId,
                        ConnectionStatus = (now - x.ServerDateTime).Minutes < trackerAddressValidMinutes
                            ? ConnectionStatusEnum.Connected
                            : ConnectionStatusEnum.NotConnected,
                        MovementStatus = x.Speed == null
                            ? MovementStatusEnum.NoData
                            : x.Speed.Value > 0
                                ? MovementStatusEnum.Moving
                                : MovementStatusEnum.Stopped,
                        LastMessageTime = x.TrackerDateTime,
                        NumberOfSatellites = x.SatNumber
                    })
                    .First())
            .ToDictionary(x => x.TrackerExternalId);

        return foundResults;
    }

    /// <summary>
    /// Информация о перемещении трекеров за выбранный период
    /// </summary>
    /// <param name="trackStartTime">Начало периода</param>
    /// <param name="trackEndTime">Конец периода</param>
    /// <param name="externalIds">Внешние id трекеров</param>
    /// <returns>Данные трека для машин за последние сутки</returns>
    public IDictionary<int, TrackPointInfo[]> GetTracks(DateTime trackStartTime,
        DateTime trackEndTime, params int[] externalIds)
    {
        if (externalIds.Length == 0)
        {
            return new Dictionary<int, TrackPointInfo[]>();
        }

        var trackStartTimeUtc = trackStartTime.ToUniversalTime();
        var trackEndTimeUtc = trackEndTime.ToUniversalTime();

        var filteredMessages = QueryableProvider.Linq()
            .Where(x => externalIds.Contains(x.ExternalTrackerId) &&
                        x.Latitude != null && x.Longitude != null &&
                        x.ServerDateTime >= trackStartTimeUtc &&
                        x.ServerDateTime <= trackEndTimeUtc)
            .OrderBy(x => x.TrackerDateTime);

        var result = filteredMessages
            .ToLookup(x => x.ExternalTrackerId,
                x => new TrackPointInfo
                {
                    MessageId = x.Id,
                    Latitude = x.Latitude!.Value,
                    Longitude = x.Longitude!.Value,
                    NumberOfSatellites = x.SatNumber,
                    Speed = x.Speed,
                    Time = x.ServerDateTime
                })
            .ToDictionary(x => x.Key, x => x.ToArray());

        return result;
    }

    public IDictionary<int, (double Lat, double Long)> GetLocations(params int[] externalIds)
    {
        var result = QueryableProvider.Linq()
            .Where(x => externalIds.Contains(x.ExternalTrackerId) &&
                        x.Latitude != null && x.Longitude != null)
            .GroupBy(x => x.ExternalTrackerId,
                (key, g) => g.OrderByDescending(x => x.ServerDateTime)
                    .Select(x => new { x.ExternalTrackerId, x.Longitude, x.Latitude })
                    .First())
            .ToDictionary(x => x.ExternalTrackerId, x => (x.Latitude!.Value, x.Longitude!.Value));

        return result;
    }

    public ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = end.ToUniversalTime();

        var messages = HydratedQuery.AsNoTracking()
            .Where(x => x.ExternalTrackerId == externalId &&
                        x.ServerDateTime >= startUtc &&
                        x.ServerDateTime <= endUtc);
        //Пробег берем из тега can_b0 - значение нужно умножить на 5
        var firstMileage = ((int?)messages.OrderBy(x => x.ServerDateTime)
            .SelectMany(x => x.Tags)
            .FirstOrDefault(x => x.TrackerTagId == TagsSeed.CanB0Id)
            ?.GetValue() ?? 0) * 5;
        
        var lastMileage = ((int?)messages.OrderByDescending(x => x.ServerDateTime)
            .SelectMany(x => x.Tags)
            .FirstOrDefault(x => x.TrackerTagId == TagsSeed.CanB0Id)
            ?.GetValue() ?? 0) * 5;

        return new ViewMessageStatisticsDto
        {
            NumberOfMessages = messages.Count(),
            MaxSpeed = messages.Where(x => x.Speed != null)
                .Select(x => x.Speed!.Value)
                .DefaultIfEmpty()
                .Max(),
            AverageSpeed = messages.Where(x => x.Speed != null && x.Speed != 0)
                .Select(x => x.Speed!.Value)
                .DefaultIfEmpty()
                .Average(),
            Distance = (lastMileage - firstMileage),
            Mileage = lastMileage,
            TotalTime = (messages.Select(x => x.ServerDateTime).DefaultIfEmpty().Max() -
                         messages.Select(x => x.ServerDateTime).DefaultIfEmpty().Min()).Seconds
        };
    }

    /// <summary>
    /// Постранично возвращает массив событий со значениями параметров трекера для зададанного трекера и временного диапазона
    /// </summary>
    /// <param name="externalId"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public PagedResult<TrackerDataMessageDto> GetParameterDataTrackerMessages(int externalId, DateTime start, DateTime end,
        int pageNum, int pageSize)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = end.ToUniversalTime();

        PagedResult<TrackerMessage> messages = QueryableProvider
            .Fetch(m => m.Tags.Where(x => x.SensorId == null))
            .Linq()
            .Where(m => m.ExternalTrackerId == externalId &&
                        m.ServerDateTime >= startUtc &&
                        m.ServerDateTime <= endUtc)
            .OrderBy(x => x.ServerDateTime)
            .AsNoTracking()
            .GetPagedQueryable(pageNum, pageSize);

        Dictionary<int, TrackerTag> trackerTags = _tagsRepository.GetTags().ToDictionary(x => x.Id);

        return new PagedResult<TrackerDataMessageDto>
        {
            Results = messages.Results.Select((m, idx) => new TrackerDataMessageDto
            {
                Id = m.Id,
                Num = (messages.CurrentPage - 1) * messages.PageSize + idx + 1,
                ServerDateTime = m.ServerDateTime,
                TrackerDateTime = m.TrackerDateTime,
                Latitude = m.Latitude,
                Longitude = m.Longitude,
                Altitude = m.Altitude,
                SatNumber = m.SatNumber,
                Speed = m.Speed,
                Parameters = GetParametersForMessage(m, trackerTags)
            }).ToList(),
            CurrentPage = messages.CurrentPage,
            PageSize = messages.PageSize,
            TotalRowCount = messages.TotalRowCount,
            TotalPageCount = messages.TotalPageCount
        };
    }

    /// <summary>
    /// Постранично возвращает  массив событий со значениями датчиков трекера для зададанного трекера и временного диапазона
    /// </summary>
    /// <param name="externalId"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public PagedResult<SensorDataMessageDto> GetSensorDataTrackerMessages(int externalId, DateTime start, DateTime end, 
        int pageNum, int pageSize)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = end.ToUniversalTime();

        PagedResult<TrackerMessage> messages = QueryableProvider
            .Fetch(m => m.Tags.Where(x => x.SensorId != null))
            .Linq()
            .Where(m => m.ExternalTrackerId == externalId &&
                        m.ServerDateTime >= startUtc &&
                        m.ServerDateTime <= endUtc)
            .OrderBy(x => x.ServerDateTime)
            .AsNoTracking()
            .GetPagedQueryable(pageNum, pageSize);

        var tracker = _trackerRepository.FindTracker(imei: null, externalId: externalId);
        if (tracker == null)
        {
            throw new ArgumentNullException($"{externalId} не привязан к трекеру");
        }

        return new PagedResult<SensorDataMessageDto>
        {
            Results = messages.Results.Select((m, idx) => new SensorDataMessageDto
            {
                Id = m.Id,
                Num = (messages.CurrentPage - 1) * messages.PageSize + idx + 1,
                ServerDateTime = m.ServerDateTime,
                TrackerDateTime = m.TrackerDateTime,
                Latitude = m.Latitude,
                Longitude = m.Longitude,
                Altitude = m.Altitude,
                SatNumber = m.SatNumber,
                Speed = m.Speed,
                Sensors = GetSensorsForMessage(m, tracker.Sensors)
            }).ToList(),
            CurrentPage = messages.CurrentPage,
            PageSize = messages.PageSize,
            TotalRowCount = messages.TotalRowCount,
            TotalPageCount = messages.TotalPageCount
        };
    }

    /// <summary>
    /// Удаляет сообщения из списка
    /// </summary>
    /// <param name="messageIds">список идентификаторов для удаления</param>
    public void DeleteMessages(long[] messageIds)
    {
        var existingFromList = QueryableProvider.Linq().Where(m => messageIds.Contains(m.Id)).ToList();
        if (messageIds.Length > existingFromList.Count())
        {
            throw new ArgumentException($"Для некоторых идентификаторов из списка не найдены сообщения");
        }

        foreach (var message in existingFromList)
        {
            base.Remove(message);
        }
    }

    private TrackerParameter[] GetParametersForMessage(TrackerMessage message, Dictionary<int, TrackerTag> trackerTags)
    {
        IList<MessageTag>? tags = message.Tags;

        if (tags is null || !tags.Any())
        {
            return new TrackerParameter[] { };
        }
        
        var result = new List<TrackerParameter>();
        foreach (var tag in tags)
        {
            var parameter = new TrackerParameter();
            if (tag.TrackerTagId.HasValue)
            {
                parameter.ParamName = trackerTags[tag.TrackerTagId.Value].Name;
            }

            switch (tag.TagType)
            {
                case TagDataTypeEnum.Integer:
                    parameter.LastValueDecimal = ((MessageTagInteger)tag).Value;
                    break;
                case TagDataTypeEnum.Bits or TagDataTypeEnum.Boolean or TagDataTypeEnum.String:
                    parameter.LastValueString = tag.ValueString;
                    break;
                case TagDataTypeEnum.Byte:
                    parameter.LastValueDecimal = ((MessageTagByte)tag).Value;
                    break;
                case TagDataTypeEnum.Double:
                    parameter.LastValueDecimal = ((MessageTagDouble)tag).Value;
                    break;
                case TagDataTypeEnum.DateTime:
                    parameter.LastValueDateTime = ((MessageTagDateTime)tag).Value;
                    break;
            }

            result.Add(parameter);
        }

        return result.ToArray();
    }

    private TrackerSensorDto[] GetSensorsForMessage(TrackerMessage message, List<Sensor> sensors)
    {
        var trackerSensors = sensors
            .Where(x => x.IsVisible)
            .ToDictionary(x => x.Id,
                x => new TrackerSensorDto
                {
                    Name = x.Name,
                    Unit = x.Unit.Name
                });

        var trackerSensorDtos = new List<TrackerSensorDto>();
        foreach (MessageTag tag in message.Tags.Where(t => t.SensorId.HasValue))
        {
            if (tag.SensorId != null && trackerSensors.TryGetValue(tag.SensorId.Value, out var sensorDto))
            {
                sensorDto.Value = tag.ValueString;
            }
        }

        return trackerSensors.Values.ToArray();
    }
}