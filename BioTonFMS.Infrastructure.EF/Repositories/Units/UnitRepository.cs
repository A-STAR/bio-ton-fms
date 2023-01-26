using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.Units
{
    public class UnitRepository : Repository<Unit>, IUnitRepository
    {
        public UnitRepository(IKeyValueProvider<Unit, int> keyValueProvider,
            IQueryableProvider<Unit> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }

        public IEnumerable<Unit> GetUnits()
        {
            var linqProvider = QueryableProvider.Linq().AsNoTracking().OrderBy(c => c.Name);
            return linqProvider.ToList();
        }
    }
}
