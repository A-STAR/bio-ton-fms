using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

public class TrackerTagRepository : Repository<TrackerTag>
{
    public TrackerTagRepository(IKeyValueProvider<TrackerTag, int> keyValueProvider,
        IQueryableProvider<TrackerTag> queryableProvider,
        UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
    }
}