using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups
{
    public class VehicleGroupRepository : Repository<VehicleGroup>, IVehicleGroupRepository
    {
        public VehicleGroupRepository(IKeyValueProvider<VehicleGroup, int> keyValueProvider,
            IQueryableProvider<VehicleGroup> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }

        public IEnumerable<VehicleGroup> GetVehicleGroups()
        {
            var linqProvider = QueryableProvider.Linq().AsNoTracking().OrderBy(c => c.Name);
            return linqProvider.ToList();
        }
    }
}
