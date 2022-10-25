using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.FuelTypes
{
    public class FuelTypeRepository : Repository<FuelType>, IFuelTypeRepository
    {
        public FuelTypeRepository(IKeyValueProvider<FuelType, int> keyValueProvider,
            IQueryableProvider<FuelType> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }

        public IEnumerable<FuelType> GetFuelTypes()
        {
            var linqProvider = QueryableProvider.Linq().AsNoTracking().OrderBy(c => c.Name);
            return linqProvider.ToList();
        }
    }
}
