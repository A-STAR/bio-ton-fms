using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
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

    public TrackerMessageRepository(IKeyValueProvider<TrackerMessage, int> keyValueProvider,
        IQueryableProvider<TrackerMessage> queryableProvider,
        UnitOfWorkFactory<MessagesDBContext> unitOfWorkFactory,
        ITrackerTagRepository tagsRepository)
        : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
        _tagsRepository = tagsRepository;
    }

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

    public IList<TrackerMessage> GetTrackerMessagesForDate(int[] trackerExternalIds, DateOnly date, bool forUpdate = false)
    {
        var query = forUpdate ? HydratedQuery : HydratedQuery.AsNoTracking();
        query = query
            .Where(m => trackerExternalIds.Contains(m.ExternalTrackerId) && DateOnly.FromDateTime(m.ServerDateTime) == date)
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

    public IList<TrackerParameter> GetParameters(int externalId)
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
            TolalRowCount = page.TolalRowCount,
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
    /// Информация о перемещении трекеров за сутки
    /// </summary>
    /// <param name="externalIds">Внешние id трекеров</param>
    /// <returns>Данные трека для машин за последние сутки</returns>
    public IDictionary<int, TrackPointInfo[]> GetTracks(DateTime trackStartTime, int[] externalIds)
    {
        if (externalIds.Length == 0)
        {
            return new Dictionary<int, TrackPointInfo[]>();
        }

        var trackStartTimeUtc = trackStartTime.ToUniversalTime();

        var result = QueryableProvider.Linq()
            .Where(x => externalIds.Contains(x.ExternalTrackerId) &&
                        x.Latitude != null && x.Longitude != null &&
                        x.ServerDateTime > trackStartTimeUtc)
            .OrderBy(x => x.TrackerDateTime)
            .ToLookup(x => x.ExternalTrackerId,
                x => new TrackPointInfo
                {
                    MessageId = x.Id,
                    Latitude = x.Latitude!.Value,
                    Longitude = x.Longitude!.Value,
                    Altitude = x.Altitude,
                    Speed = x.Speed,
                    Time = x.ServerDateTime
                })
            .ToDictionary(x => x.Key, x => x.ToArray());

        return result;
    }
    
    public IDictionary<int, (double Lat, double Long)> GetLocations(int[] externalIds)
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
}