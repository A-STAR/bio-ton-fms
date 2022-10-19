using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        public IEnumerable<Device> GetAllDevices(int id);
        public void RemoveConstraintVehicleByTrackerId(int trackerId);
    }
}
