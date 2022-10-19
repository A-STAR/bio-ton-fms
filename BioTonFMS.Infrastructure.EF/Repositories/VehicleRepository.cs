using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;

namespace BioTonFMS.Infrastructure.EF.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(IKeyValueProvider<Vehicle, int> keyValueProvider,
            IQueryableProvider<Vehicle> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory): base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }

        IEnumerable<Device> IVehicleRepository.GetAllDevices(int id)
        {
            var vehicle = this.QueryableProvider
                .Fetch(v => v.Tracker).ThenFetch(t => t.Devices).Linq().Where(v => v.Id == id).Single();
            var tracker = vehicle.Tracker;
            var devices = tracker.Devices;
            return devices;
        }

        // TODO: Убрать, если каскадное удаление. Если удаляем связи в ручную, добавить try/catch + проверки
        public void RemoveConstraintVehicleByTrackerId(int trackerId)
        {
            var vehicle = this.QueryableProvider
                .Linq().Where(v => v.TrackerId == trackerId).FirstOrDefault();

            if (vehicle is not null)
            {
                vehicle.TrackerId = null;
                Update(vehicle);
            }
        }
    }
}
