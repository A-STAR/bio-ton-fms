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
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(IKeyValueProvider<Vehicle, int> keyValueProvider,
            IQueryableProvider<Vehicle> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }

        public new Vehicle? this[int key]
        {
            get
            {
                var vehicle = QueryableProvider
                    .Fetch(v => v.VehicleGroup)
                    .Fetch(v => v.FuelType)
                    .Fetch(v => v.Tracker)
                    .Linq()
                    .SingleOrDefault(v => v.Id == key);
                return vehicle;
            }
        }

        public PagedResult<Vehicle> GetVehicles(VehiclesFilter filter)
        {
            var linqProvider = QueryableProvider
                .Fetch(v => v.VehicleGroup)
                .Fetch(v => v.FuelType)
                .Fetch(v => v.Tracker).Linq();

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

            var vehicles = vehiclePredicate != null ?
                linqProvider.Where(vehiclePredicate) :
                linqProvider;

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
                        vehicles = SetSortDirection(filter, vehicles, v => v.VehicleGroup.Name);
                        break;
                    case VehicleSortBy.SubType:
                        vehicles = SetSortDirection(filter, vehicles, v => (int)v.VehicleSubType);
                        break;
                    case VehicleSortBy.FuelType:
                        vehicles = SetSortDirection(filter, vehicles, v => v.FuelType.Name);
                        break;
                }
            }

            return vehicles.AsNoTracking().GetPagedQueryable(
                 filter.PageNum, filter.PageSize);
        }

        public override void Add(Vehicle vehicle)
        {
            var vehicleWithTheSameName = QueryableProvider.Linq().Where(v => v.Name == vehicle.Name);
            if (vehicleWithTheSameName.Any())
            {
                throw new ArgumentException($"Машина с именем {vehicle.Name} уже существует");
            }
            base.Add(vehicle);
        }

        public override void Update(Vehicle vehicle)
        {
            var vehicleWithTheSameName = QueryableProvider.Linq()
                .Where(v => v.Name == vehicle.Name && v.Id != vehicle.Id);
            if (vehicleWithTheSameName.Any())
            {
                throw new ArgumentException($"Машина с именем {vehicle.Name} уже существует");
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
