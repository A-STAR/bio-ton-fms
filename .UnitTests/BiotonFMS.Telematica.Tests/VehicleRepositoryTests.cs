using AutoMapper;
using BiotonFMS.Telematica.Tests.Mocks;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF.Providers;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Mapping;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BiotonFMS.Telematica.Tests
{
    public class VehicleRepositoryTests
    {
        [Fact]
        public void VehicleRepository_AddShouldThrowException_IfVehicleWithSuchNameExists()
        {
            // Arrange
            var existingVehicle = new Vehicle()
            {
                Id = 1,
                Name = "Сущесвующая",
                Type = VehicleTypeEnum.Transport,
                VehicleSubType = VehicleSubTypeEnum.Car,
                FuelType = new FuelType { Id = 1, Name = "Бензин" },
                FuelTypeId = 1,
                Description = "Описание 1",
                Make = "Ford",
                Model = "Focus",
                ManufacturingYear = 2020,
                RegistrationNumber = "В167АР 199",
                InventoryNumber = "1234"
            };
            var vehicleList = new List<Vehicle>();
            vehicleList.Add(existingVehicle);

            VehicleRepository repo = CreateVehicleRepository(vehicleList);

            var newVehicle = new Vehicle()
            {
                Name = "Сущесвующая",
                Type = VehicleTypeEnum.Transport,
                VehicleSubType = VehicleSubTypeEnum.Car,
                FuelType = new FuelType { Id = 1, Name = "Бензин" },
                FuelTypeId = 1,
                Description = "Описание 1",
                Make = "Ford",
                Model = "Mondeo",
                ManufacturingYear = 2019,
                RegistrationNumber = "В165АР 199",
                InventoryNumber = "1235"
            };

            // Act
            repo.Invoking(r => r.Add(newVehicle)).Should().Throw<ArgumentException>()
                .WithMessage($"Машина с именем {existingVehicle.Name} уже существует");
            // Assert
        }

        private static VehicleRepository CreateVehicleRepository(List<Vehicle> vehicleList)
        {
            IKeyValueProvider<Vehicle, int> keyValueProviderMock = new KeyValueProviderMock<Vehicle, int>(vehicleList);
            IQueryableProvider<Vehicle> vehicleQueryProviderMock = new QueryableProviderMock<Vehicle>(vehicleList);
            UnitOfWorkFactory unitOfWorkFactoryMock = new UnitOfWorkFactoryMock();

            var repo = new VehicleRepository(keyValueProviderMock, vehicleQueryProviderMock, unitOfWorkFactoryMock);
            return repo;
        }
    }
}
