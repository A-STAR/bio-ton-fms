using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.Units
{
    public interface IUnitRepository : IRepository<Unit>
    {
        IEnumerable<Unit> GetUnits();
    }
}
