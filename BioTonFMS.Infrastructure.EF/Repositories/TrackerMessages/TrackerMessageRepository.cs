using System.Collections;
using System.Text;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
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

    public override void Add(TrackerMessage entity)
    {
        entity.ServerDateTime = SystemTime.UtcNow;
        base.Add(entity);
    }

    public IList<TrackerMessage> GetMessagesForUpdate()
    {
        var linqProvider = QueryableProvider
            .Fetch(m => m.Tags)
            .Linq()
            .OrderBy(m => m.TrId)
            .ThenBy(m => m.Id);
        return linqProvider.ToList();
    }

    public IList<TrackerMessage> GetMessages()
    {
        var linqProvider = QueryableProvider.Fetch(m => m.Tags).Linq().AsNoTracking().OrderBy(m => m.Id);
        return linqProvider.ToList();
    }

    public bool ExistsByUID(Guid uid) =>
        QueryableProvider.Linq().Any(x => x.PackageUID == uid);

    public TrackerStandardParameters GetStandardParameters(int externalId, string imei)
    {
        var stdParams = new TrackerStandardParameters();

        var last = QueryableProvider.Linq()
            .Where(x => x.TrId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault();

        if (last is null) return stdParams;

        stdParams.Time = last.ServerDateTime;

        stdParams.Long = last.Longitude ?? QueryableProvider.Linq()
            .Where(x => x.Longitude != null && x.TrId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Longitude;

        stdParams.Lat = last.Latitude ?? QueryableProvider.Linq()
            .Where(x => x.Latitude != null && x.TrId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Latitude;

        stdParams.Speed = last.Speed ?? QueryableProvider.Linq()
            .Where(x => x.Speed != null && x.TrId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Speed;

        stdParams.Alt = last.Height ?? QueryableProvider.Linq()
            .Where(x => x.Height != null && x.TrId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Height;

        return stdParams;
    }

    public IList<TrackerParameter> GetParameters(int externalId, string imei)
    {
        var lastTags = QueryableProvider.Fetch(x => x.Tags).Linq()
            .Where(x => x.TrId == externalId || x.Imei == imei)
            .OrderByDescending(x => x.ServerDateTime)
            .FirstOrDefault()?.Tags;

        if (lastTags is null || !lastTags.Any())
        {
            return new List<TrackerParameter>();
        }

        var trackerTags = _tagsRepository.GetTags().ToDictionary(x => x.Id);
        var result = new List<TrackerParameter>();
        foreach (var t in lastTags)
        {
            var tp = new TrackerParameter();
            if (t.TrackerTagId.HasValue)
            {
                tp.ParamName = trackerTags[t.TrackerTagId.Value].Name;
            }

            switch (t.TagType)
            {
                case TagDataTypeEnum.Integer:
                    tp.LastValueDecimal = ((MessageTagInteger)t).Value;
                    break;
                case TagDataTypeEnum.Bits:
                    tp.LastValueString = GetBitString(((MessageTagBits)t).Value);
                    break;
                case TagDataTypeEnum.Byte:
                    tp.LastValueDecimal = ((MessageTagByte)t).Value;
                    break;
                case TagDataTypeEnum.Double:
                    tp.LastValueDecimal = ((MessageTagDouble)t).Value;
                    break;
                case TagDataTypeEnum.Boolean:
                    tp.LastValueString = ((MessageTagBoolean)t).Value.ToString();
                    break;
                case TagDataTypeEnum.String:
                    tp.LastValueString = ((MessageTagString)t).Value;
                    break;
                case TagDataTypeEnum.DateTime:
                    tp.LastValueDateTime = ((MessageTagDateTime)t).Value;
                    break;
            }
            
            result.Add(tp);
        }

        return result;
    }
    
    private static string GetBitString(BitArray bits)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < bits.Count; i++)
        {
            char c = bits[i] ? '1' : '0';
            sb.Append(c);
        }

        return sb.ToString();
    }
}