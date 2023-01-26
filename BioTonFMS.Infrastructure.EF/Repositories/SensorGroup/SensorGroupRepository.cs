using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.SensorGroups
{
    public class SensorGroupRepository : Repository<SensorGroup>, ISensorGroupRepository
    {
        public SensorGroupRepository(IKeyValueProvider<SensorGroup, int> keyValueProvider,
            IQueryableProvider<SensorGroup> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }

        public IEnumerable<SensorGroup> GetSensorGroups()
        {
            var linqProvider = QueryableProvider.Linq().AsNoTracking().OrderBy(c => c.Name);
            return linqProvider.ToList();
        }
    }
}
