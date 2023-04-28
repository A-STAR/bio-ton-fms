using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
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

    public bool ExistsByUid(Guid uid) =>
        QueryableProvider.Linq().AsNoTracking().Any(x => x.PackageUID == uid);

    public TrackerStandardParameters GetStandardParameters(int externalId, string imei)
    {
        var stdParams = new TrackerStandardParameters();

        var last = QueryableProvider.Linq().AsNoTracking()
            .Where(x => x.ExternalTrackerId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault();

        if (last is null) return stdParams;

        stdParams.Time = last.ServerDateTime;

        stdParams.Long = last.Longitude ?? QueryableProvider.Linq()
            .Where(x => x.Longitude != null && x.ExternalTrackerId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Longitude;

        stdParams.Lat = last.Latitude ?? QueryableProvider.Linq()
            .Where(x => x.Latitude != null && x.ExternalTrackerId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Latitude;

        stdParams.Speed = last.Speed ?? QueryableProvider.Linq()
            .Where(x => x.Speed != null && x.ExternalTrackerId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Speed;

        stdParams.Alt = last.Altitude ?? QueryableProvider.Linq()
            .Where(x => x.Altitude != null && x.ExternalTrackerId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Altitude;

        return stdParams;
    }

    public IList<TrackerParameter> GetParameters(int externalId, string imei)
    {
        if (string.IsNullOrEmpty(imei) && externalId == 0)
        {
            return new List<TrackerParameter>();
        }
        
        IList<MessageTag>? lastTags = HydratedQuery
            .AsNoTracking()
            .Where(x => x.ExternalTrackerId == externalId || x.Imei == imei)
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

    public TrackerMessage? GetLastMessageFor(TrackerMessage trackerMessage)
    {
        if (string.IsNullOrEmpty(trackerMessage.Imei) && trackerMessage.ExternalTrackerId == 0)
            return null;
        
        var query = HydratedQuery
            .AsNoTracking();

        query = trackerMessage.ExternalTrackerId == 0 
            ? query.Where(m => m.Imei == trackerMessage.Imei) 
            : query.Where(m => m.ExternalTrackerId == trackerMessage.ExternalTrackerId);
        
        var lastMessage = query
            .OrderByDescending(m => m.Id)
            .FirstOrDefault();
        
        return lastMessage;
    }

    public PagedResult<ParametersHistoryRecord> GetParametersHistory(ParametersHistoryFilter filter)
    {
        Dictionary<int, string> tagNames = _tagsRepository.GetTags()
            .ToDictionary(x => x.Id, x => x.Name);
        
        var page = HydratedQuery
            .AsNoTracking()
            .Where(x => x.Imei == filter.Imei || x.ExternalTrackerId == filter.ExternalId)
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
}
