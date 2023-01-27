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
        
        public IQueryable<SensorType> HydratedQuery =>
            QueryableProvider
                .Fetch(v => v.SensorGroup)   
                .Fetch(v => v.Unit)
                .Linq();

        public new SensorType? this[int id] => HydratedQuery.SingleOrDefault(st => st.Id == id); 

        public IEnumerable<SensorType> GetSensorTypes()
        {
            var linqProvider = HydratedQuery.AsNoTracking().OrderBy(c => c.Name);
            return linqProvider.ToList();
        }
    }
}
