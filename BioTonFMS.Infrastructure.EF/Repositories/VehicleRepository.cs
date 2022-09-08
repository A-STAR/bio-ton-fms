using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using LinqSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
