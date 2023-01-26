using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.SensorTypes
{
    public class SensorTypeRepository : Repository<SensorType>, ISensorTypeRepository
    {
        public SensorTypeRepository(IKeyValueProvider<SensorType, int> keyValueProvider,
            IQueryableProvider<SensorType> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }

        public IEnumerable<SensorType> GetSensorTypes()
        {
            var linqProvider = QueryableProvider.Linq().AsNoTracking().OrderBy(c => c.Name);
            return linqProvider.ToList();
        }
    }
}
