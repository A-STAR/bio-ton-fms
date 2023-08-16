using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;

namespace BiotonFMS.Telematica.Tests.Mocks.Repositories;

public static class VehicleRepositoryMock
{
    public static VehicleRepository GetStub(ICollection<Vehicle>? vehicleList = null)
    {
        vehicleList ??= SampleVehicles;
        
        IKeyValueProvider<Vehicle, int> keyValueProviderMock = new KeyValueProviderMock<Vehicle, int>(vehicleList);
        IQueryableProvider<Vehicle> vehicleQueryProviderMock = new QueryableProviderMock<Vehicle>(vehicleList);
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactoryMock = new BioTonDBContextUnitOfWorkFactoryMock();

        var repo = new VehicleRepository(keyValueProviderMock, vehicleQueryProviderMock, unitOfWorkFactoryMock);
        return repo;
    }

    public static IList<Vehicle> SampleVehicles => new List<Vehicle>
    {
        new()
        {
            Id = 1,
            Name = "Красная машина",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            Description = "Описание 1",
            Make = "Ford",
            Model = "Focus",
            ManufacturingYear = 2020,
            RegistrationNumber = "В167АР 199",
            InventoryNumber = "1234",
            TrackerId = 1,
            Tracker = new Tracker
            {
                Id = 1,
                Imei = "123",
                ExternalId = 2552
            }
        },
        new()
        {
            Id = 2,
            Name = "Синяя машина",
            Type = VehicleTypeEnum.Agro,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            VehicleGroup = new VehicleGroup { Id = 1, Name = "Группа 1" },
            VehicleGroupId = 1,
            Description = "Описание 2",
            Make = "Ford",
            Model = "Focus",
            ManufacturingYear = 2015,
            RegistrationNumber = "В167АР 172",
            InventoryNumber = "1235",
            TrackerId = 1,
            Tracker = new Tracker
            {
                Id = 2,
                Imei = "128128",
                ExternalId = 15
            }
        },
        new()
        {
            Id = 3,
            Name = "Желтая машина",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Sprayer,
            FuelType = new FuelType { Id = 2, Name = "Дизель" },
            FuelTypeId = 2,
            VehicleGroup = new VehicleGroup { Id = 2, Name = "Группа 2" },
            VehicleGroupId = 2,
            Description = "Описание 3",
            Make = "Mazda",
            Model = "CX5",
            ManufacturingYear = 2010,
            RegistrationNumber = "В167АР 174",
            InventoryNumber = "1236",
            TrackerId = 1,
            Tracker = new Tracker
            {
                Id = 3,
                Imei = "64128256",
                ExternalId = 128
            }
        }
    };
}