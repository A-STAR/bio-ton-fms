using BioTonFMS.Domain;

namespace BioTonFMS.Infrastructure.EF.Repositories.FuelTypes
{
    public interface IFuelTypeRepository
    {
        IEnumerable<FuelType> GetFuelTypes();
    }
}
