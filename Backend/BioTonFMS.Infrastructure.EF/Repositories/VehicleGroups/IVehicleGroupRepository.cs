using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups
{
    public interface IVehicleGroupRepository : IRepository<VehicleGroup>
    {
        IEnumerable<VehicleGroup> GetVehicleGroups();
    }
}
