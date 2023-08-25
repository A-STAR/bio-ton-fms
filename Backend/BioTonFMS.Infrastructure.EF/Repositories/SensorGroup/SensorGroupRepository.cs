using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.SensorGroups
{
    public class SensorGroupRepository : Repository<SensorGroup, BioTonDBContext>, ISensorGroupRepository
    {
        public SensorGroupRepository(IKeyValueProvider<SensorGroup, int> keyValueProvider,
            IQueryableProvider<SensorGroup> queryableProvider,
            UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }

        public IEnumerable<SensorGroup> GetSensorGroups()
        {
            var linqProvider = QueryableProvider
                .Linq()
                .Include(s => s.SensorTypes)
                .AsNoTracking()
                .OrderBy(c => c.Id);
            return linqProvider.ToList();
        }
    }
}
