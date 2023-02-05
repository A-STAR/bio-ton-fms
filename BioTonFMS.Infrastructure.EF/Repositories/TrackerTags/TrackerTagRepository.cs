using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

public class TrackerTagRepository : Repository<TrackerTag, BioTonDBContext>, ITrackerTagRepository
{
    public TrackerTagRepository(IKeyValueProvider<TrackerTag, int> keyValueProvider,
        IQueryableProvider<TrackerTag> queryableProvider,
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
    }

    public IEnumerable<TrackerTag> GetTagsForTrackerType(TrackerTypeEnum trackerType)
    {
        var tags = QueryableProvider
            .Fetch(x => x.ProtocolTags)
            .Linq()
            .Where(x => x.ProtocolTags.Any(t => t.TrackerType == trackerType))
            .ToList();
        
        return tags;
    }

    public TrackerTag? GetTagForTrackerType(TrackerTypeEnum trackerType, int protocolTagCode)
    {
        
        var tag = QueryableProvider
            .Fetch(x => x.ProtocolTags)
            .Linq()
            .SingleOrDefault(x => x.ProtocolTags.Any(t => t.TrackerType == trackerType &&
                                                          t.ProtocolTagCode == protocolTagCode));
        return tag;
    }
}