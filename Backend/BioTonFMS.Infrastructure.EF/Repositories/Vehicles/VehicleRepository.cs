using System.Linq.Expressions;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Paging.Extensions;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BioTonFMS.Infrastructure.Utils.Builders;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.Vehicles
{
    public class VehicleRepository : Repository<Vehicle, BioTonDBContext>, IVehicleRepository
    {
        public VehicleRepository(
            IKeyValueProvider<Vehicle, int> keyValueProvider,
            IQueryableProvider<Vehicle> queryableProvider,
            UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactory) : base(
            keyValueProvider,
            queryableProvider,
            unitOfWorkFactory)
        {
        }

        public new Vehicle? this[int key]
        {
            get
            {
                var vehicle = HydratedQuery
                    .SingleOrDefault(v => v.Id == key);
                return vehicle;
            }
        }

        private IQueryable<Vehicle> HydratedQuery
        {
            get
            {
                return QueryableProvider
                    .Fetch(v => v.VehicleGroup)
                    .Fetch(v => v.FuelType)
                    .Fetch(v => v.Tracker)
                    .Linq();
            }
        }

        public PagedResult<Vehicle> GetVehicles(VehiclesFilter filter, bool hydrate = true)
        {
            var query = hydrate ? HydratedQuery : QueryableProvider.Linq();

            Expression<Func<Vehicle, bool>>? vehiclePredicate = null;

            if (!string.IsNullOrEmpty(filter.Name))
            {
                vehiclePredicate = vehicle => vehicle.Name == filter.Name;
            }

            if (filter.Type.HasValue)
            {
                Expression<Func<Vehicle, bool>>? typePredicate =
                    vehicle => vehicle.Type == filter.Type;

                vehiclePredicate = SetPredicate(vehiclePredicate, typePredicate);
            }

            if (filter.GroupId.HasValue)
            {
                Expression<Func<Vehicle, bool>>? groupPredicate =
                    vehicle => vehicle.VehicleGroupId == filter.GroupId;

                vehiclePredicate = SetPredicate(vehiclePredicate, groupPredicate);
            }

            if (filter.SubType.HasValue)
            {
                Expression<Func<Vehicle, bool>>? subTypePredicate =
                    vehicle => vehicle.VehicleSubType == filter.SubType;

                vehiclePredicate = SetPredicate(vehiclePredicate, subTypePredicate);
            }

            if (filter.FuelTypeId.HasValue)
            {
                Expression<Func<Vehicle, bool>>? fuelTypePredicate =
                    vehicle => vehicle.FuelTypeId == filter.FuelTypeId;

                vehiclePredicate = SetPredicate(vehiclePredicate, fuelTypePredicate);
            }

            if (!string.IsNullOrEmpty(filter.RegNum))
            {
                Expression<Func<Vehicle, bool>>? regNumPredicate =
                    vehicle => vehicle.RegistrationNumber == filter.RegNum;

                vehiclePredicate = SetPredicate(vehiclePredicate, regNumPredicate);
            }

            var vehicles = vehiclePredicate is not null ? query.Where(vehiclePredicate) : query;

            if (filter.SortBy.HasValue)
            {
                switch (filter.SortBy)
                {
                    case VehicleSortBy.Name:
                        vehicles = SetSortDirection(filter, vehicles, v => v.Name);
                        break;
                    case VehicleSortBy.Type:
                        vehicles = SetSortDirection(filter, vehicles, v => (int)v.Type);
                        break;
                    case VehicleSortBy.Group:
                        vehicles = SetSortDirection(filter, vehicles,
                            v => !v.VehicleGroupId.HasValue ? "" : v.VehicleGroup!.Name);
                        break;
                    case VehicleSortBy.SubType:
                        vehicles = SetSortDirection(filter, vehicles, v => (int)v.VehicleSubType);
                        break;
                    case VehicleSortBy.FuelType:
                        vehicles = SetSortDirection(filter, vehicles, v => v.FuelType.Name);
                        break;
                }
            }

            return vehicles.AsNoTracking()
                .GetPagedQueryable(filter.PageNum, filter.PageSize);
        }

        public Vehicle[] FindVehicles(string? findCriterion)
        {
            // На странице мониторинга показываем только машины с привязанными трекерами
            var vehicleQuery = QueryableProvider.Fetch(x => x.Tracker).Linq().Where(x => x.Tracker != null).OrderBy(x => x.Name);
            return (string.IsNullOrEmpty(findCriterion)
                ? vehicleQuery
                : vehicleQuery.Where(x => x.Name.ToLower().Contains(findCriterion.ToLower()) ||
                                (x.Tracker != null &&
                                    (x.Tracker.ExternalId.ToString() == findCriterion ||
                                     x.Tracker.Imei == findCriterion)))
            ).ToArray();
        }

        public IDictionary<int, int> GetExternalIds(params int[] vehicleIds) =>
            QueryableProvider.Fetch(x => x.Tracker)
                .Linq()
                .Where(x => vehicleIds.Contains(x.Id) && x.Tracker != null)
                .ToDictionary(x => x.Id, x => x.Tracker!.ExternalId);

        public IDictionary<int, string> GetNames(int[] vehicleIds) =>
            QueryableProvider.Fetch(x => x.Tracker)
                .Linq()
                .Where(x => vehicleIds.Contains(x.Id) && x.Tracker != null)
                .ToDictionary(x => x.Id, x => x.Name);

        public override void Add(Vehicle vehicle)
        {
            var vehicleWithTheSameName = QueryableProvider.Linq().Where(v => v.Name == vehicle.Name);
            if (vehicleWithTheSameName.Any())
            {
                throw new ArgumentException($"Машина с именем {vehicle.Name} уже существует");
            }

            if (vehicle.TrackerId.HasValue)
            {
                var sameTrackerVehicle = QueryableProvider.Fetch(x => x.Tracker).Linq()
                    .FirstOrDefault(x => x.TrackerId == vehicle.TrackerId);
                if (sameTrackerVehicle is not null)
                {
                    throw new ArgumentException(
                        $"Трекер {sameTrackerVehicle.Tracker!.Name} уже используется для машины {sameTrackerVehicle.Name}");
                }
            }

            base.Add(vehicle);
        }

        public Tracker? GetTracker(int vehicleId) => QueryableProvider
            .Linq()
            .Include(x => x.Tracker)
            .ThenInclude(x => x.Sensors)
            .ThenInclude(x => x.Unit)
            .Where(x => x.Id == vehicleId && x.Tracker != null)
            .Select(x => x.Tracker)
            .FirstOrDefault();

        public override void Update(Vehicle vehicle)
        {
            var vehicleWithTheSameName = QueryableProvider.Linq()
                .Where(v => v.Name == vehicle.Name && v.Id != vehicle.Id);
            if (vehicleWithTheSameName.Any())
            {
                throw new ArgumentException($"Машина с именем {vehicle.Name} уже существует");
            }

            if (vehicle.TrackerId.HasValue)
            {
                var sameTrackerVehicle = QueryableProvider.Fetch(x => x.Tracker).Linq()
                    .FirstOrDefault(x => x.TrackerId == vehicle.TrackerId && x.Id != vehicle.Id);
                if (sameTrackerVehicle is not null)
                {
                    throw new ArgumentException(
                        $"Трекер {sameTrackerVehicle.Tracker!.Name} уже используется для машины {sameTrackerVehicle.Name}");
                }
            }

            base.Update(vehicle);
        }

        // TODO: сделать один метод для repository
        private static IQueryable<Vehicle> SetSortDirection<TKey>(
            VehiclesFilter filter,
            IQueryable<Vehicle> vehicles,
            Expression<Func<Vehicle, TKey>> property)
        {
            switch (filter.SortDirection)
            {
                case SortDirection.Ascending:
                    vehicles = vehicles.OrderBy(property);
                    break;
                case SortDirection.Descending:
                    vehicles = vehicles.OrderByDescending(property);
                    break;
            }

            return vehicles;
        }

        private static Expression<Func<Vehicle, bool>>? SetPredicate(
            Expression<Func<Vehicle, bool>>? vehiclePredicate,
            Expression<Func<Vehicle, bool>> customPredicate)
        {
            if (vehiclePredicate == null)
            {
                vehiclePredicate = customPredicate;
            }
            else
            {
                vehiclePredicate = vehiclePredicate?.And(customPredicate);
            }

            return vehiclePredicate;
        }
    }
}