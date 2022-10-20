using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.Vehicles
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        IEnumerable<Device> GetAllDevices(int id);
        PagedResult<Vehicle> GetVehicles(VehiclesFilter filter);
    }
}
