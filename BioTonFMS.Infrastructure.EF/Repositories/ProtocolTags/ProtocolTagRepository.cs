using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;

namespace BioTonFMS.Infrastructure.EF.Repositories.ProtocolTags;

public class ProtocolTagRepository : Repository<ProtocolTag, BioTonDBContext>, IProtocolTagRepository
{
    public ProtocolTagRepository(IKeyValueProvider<ProtocolTag, int> keyValueProvider,
        IQueryableProvider<ProtocolTag> queryableProvider,
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
    }

    public IEnumerable<ProtocolTag> GetTagsForTrackerType(TrackerTypeEnum trackerType)
    {
        var tags = QueryableProvider
            .Fetch(x => x.Tag)
            .Linq()
            .Where(x => x.TrackerType == trackerType)
            .ToList();
        
        return tags;
    }
}