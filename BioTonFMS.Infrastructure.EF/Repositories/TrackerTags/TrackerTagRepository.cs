using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

public class TrackerTagRepository : Repository<TrackerTag>, ITrackerTagRepository
{
    public TrackerTagRepository(IKeyValueProvider<TrackerTag, int> keyValueProvider,
        IQueryableProvider<TrackerTag> queryableProvider,
        UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
    }

    public IEnumerable<TagDto> GetTagsForTrackerType(TrackerTypeEnum trackerType)
    {
        var tags = QueryableProvider
            .Fetch(x => x.ProtocolTag)
            .Linq()
            .Where(x => x.ProtocolTag.TrackerType == trackerType)
            .Select(x => new TagDto
            {
                Id = x.Id,
                ProtocolTagCode = x.ProtocolTag.ProtocolTagCode,
                TrackerType = x.ProtocolTag.TrackerType,
                Description = x.Description,
                Name = x.Name,
                DataType = x.DataType,
                StructType = x.StructType
            })
            .ToList();

        return tags;
    }

    public TagDto? GetTagForTrackerType(TrackerTypeEnum trackerType, int protocolTagCode)
    {
        var tag = QueryableProvider
            .Fetch(x => x.ProtocolTag)
            .Linq()
            .Where(x => x.ProtocolTag.TrackerType == trackerType &&
                        x.ProtocolTag.ProtocolTagCode == protocolTagCode)
            .Select(x => new TagDto
            {
                Id = x.Id,
                ProtocolTagCode = x.ProtocolTag.ProtocolTagCode,
                TrackerType = x.ProtocolTag.TrackerType,
                Description = x.Description,
                Name = x.Name,
                DataType = x.DataType,
                StructType = x.StructType
            })
            .SingleOrDefault();

        return tag;
    }
}