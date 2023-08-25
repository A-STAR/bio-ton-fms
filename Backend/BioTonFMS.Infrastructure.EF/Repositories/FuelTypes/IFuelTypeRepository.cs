using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.FuelTypes
{
    public interface IFuelTypeRepository : IRepository<FuelType>
    {
        IEnumerable<FuelType> GetFuelTypes();
    }
}
