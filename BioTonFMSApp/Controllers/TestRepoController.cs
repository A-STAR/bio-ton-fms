using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF.Repositories;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioTonApp.Controllers
{
    [Authorize]
    [ApiController]
    public class TestRepoController : ControllerBase
    {

        private readonly ILogger<TestRepoController> _logger;
        private readonly IQueryableProvider<Tracker> _trackerProvider;
        private readonly IRepository<Tracker> _trackerRepo;
        private readonly IQueryableProvider<Vehicle> _vehicleProvider;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly UnitOfWorkFactory _unitOfWorkFactory;

        public TestRepoController(
            ILogger<TestRepoController> logger,
            IQueryableProvider<Tracker> trackerProvider,
            IRepository<Tracker> trackerRepo,
            IQueryableProvider<Vehicle> vehicleProvider,
            IVehicleRepository vehicleRepo,
            UnitOfWorkFactory unitOfWorkFactory)
        {

            _logger = logger;
            _trackerProvider = trackerProvider;
            _trackerRepo = trackerRepo;
            _vehicleProvider = vehicleProvider;
            _vehicleRepo = vehicleRepo;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpGet("testRepo/GetTrackers")]
        public IEnumerable<Tracker> GetTrackers()
        {
            var trackerList = _trackerProvider.Fetch(t => t.Devices).Linq();
            return trackerList;
        }

        [HttpGet("testRepo/GetVehicles")]
        public IEnumerable<Vehicle> GetVehicles()
        {
            var vehicleList = _vehicleProvider.Fetch(v => v.Tracker).Linq().ToList();
            return vehicleList;
        }

        [HttpGet("testRepo/GetVehiclesWithDevices")]
        public IEnumerable<Vehicle> GetVehiclesWithDevices()
        {
            var vehicleList = _vehicleProvider.Fetch(v => v.Tracker).ThenFetch(t => t.Devices).Linq().ToList();
            return vehicleList;
        }

        [HttpGet("testRepo/VehicleDevices/{id}")]
        public IEnumerable<Device> GetVehicleDevices(int id)
        {
            var devices = _vehicleRepo.GetAllDevices(id); 
            return devices;
        }


        [HttpPost("testRepo/AddTracker")]
        public IEnumerable<Tracker> AddTracker(string name, string imei)
        {
            var tracker = new Tracker { Name = name, Imei = imei };
            _trackerRepo.Put(tracker);
            var trackerList = _trackerProvider.Linq();
            return trackerList;
        }

        [HttpDelete("testRepo/RemoveTracker")]
        public IEnumerable<Tracker> RemoveTracker(int id)
        {
            var tracker = _trackerRepo[id];
            if (tracker != null)
            {
                _trackerRepo.Remove(tracker);
            }
            var trackerList = _trackerProvider.Linq();
            return trackerList;
        }

        [HttpPost("testRepo/AddVehicle")]
        public IEnumerable<Vehicle> AddVehicle(string vehicleName, string trackerName, string imei)
        {
            using (_unitOfWorkFactory.GetUnitOfWork())
            {
                var tracker = new Tracker { Name = trackerName, Imei = imei };
                _trackerRepo.Put(tracker);
                var vehicle = new Vehicle { Name = vehicleName, Tracker = tracker };
                _vehicleRepo.Put(vehicle);
            }
            var vehicleList = _vehicleProvider.Linq();
            return vehicleList;
        }

    }
}