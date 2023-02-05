using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using FluentAssertions;
using Xunit.Abstractions;
using BioTonFMS.Infrastructure.EF;

namespace BiotonFMS.Telematica.Tests.RepoTests;

public class VehicleRepositoryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public VehicleRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #region Filters

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[]
            {
                "Name filter",
                new VehiclesFilter { Name = "Красная машина" },
                SampleVehicles.Where(x => x.Name == "Красная машина").ToList()
            },
            new object[]
            {
                "Type filter",
                new VehiclesFilter { Type = VehicleTypeEnum.Transport },
                SampleVehicles.Where(x => x.Type == VehicleTypeEnum.Transport).ToList()
            },
            new object[]
            {
                "Group filter",
                new VehiclesFilter { GroupId = 1 },
                SampleVehicles.Where(x => (x.VehicleGroupId ?? 0) == 1).ToList()
            },
            new object[]
            {
                "Subtype filter",
                new VehiclesFilter { SubType = VehicleSubTypeEnum.Car },
                SampleVehicles.Where(x => x.VehicleSubType == VehicleSubTypeEnum.Car).ToList()
            },
            new object[]
            {
                "Name asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Name, SortDirection = SortDirection.Ascending },
                SampleVehicles.OrderBy(x => x.Name).ToList(),
                true
            },
            new object[]
            {
                "Group asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Group, SortDirection = SortDirection.Ascending },
                SampleVehicles.OrderBy(x => x.VehicleGroup?.Name).ToList(),
                true
            },
            new object[]
            {
                "Type asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Type, SortDirection = SortDirection.Ascending },
                SampleVehicles.OrderBy(x => x.Type).ToList(),
                true
            },
            new object[]
            {
                "Fuel type asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.FuelType, SortDirection = SortDirection.Ascending },
                SampleVehicles.OrderBy(x => x.FuelType.Name).ToList(),
                true
            },
            new object[]
            {
                "Subtype asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.SubType, SortDirection = SortDirection.Ascending },
                SampleVehicles.OrderBy(x => x.VehicleSubType).ToList(),
                true
            },
            new object[]
            {
                "Name desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Name, SortDirection = SortDirection.Descending },
                SampleVehicles.OrderByDescending(x => x.Name).ToList(),
                true
            },
            new object[]
            {
                "Group desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Group, SortDirection = SortDirection.Descending },
                SampleVehicles.OrderByDescending(x => x.VehicleGroup?.Name).ToList(),
                true
            },
            new object[]
            {
                "Type desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Type, SortDirection = SortDirection.Descending },
                SampleVehicles.OrderByDescending(x => x.Type).ToList(),
                true
            },
            new object[]
            {
                "Fuel type desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.FuelType, SortDirection = SortDirection.Descending },
                SampleVehicles.OrderByDescending(x => x.FuelType.Name).ToList(),
                true
            },
            new object[]
            {
                "Subtype desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.SubType, SortDirection = SortDirection.Descending },
                SampleVehicles.OrderByDescending(x => x.VehicleSubType).ToList(),
                true
            }
        };

    [Theory, MemberData(nameof(Data))]
    public void GetVehicles_WithFilters_ShouldFilter(string testName, VehiclesFilter filter,
        List<Vehicle> expected, bool considerOrder = false)
    {
        _testOutputHelper.WriteLine(testName);
            
        var results = CreateVehicleRepository(SampleVehicles).GetVehicles(filter).Results;

        Assert.Equal(results.Count, expected.Count);

        if (considerOrder)
            results.Should().Equal(expected);
        else
            results.Should().BeEquivalentTo(expected);
    }

    #endregion

    [Fact]
    public void AddVehicle_VehicleWithSuchNameExists_ShouldThrowException()
    {
        var existingVehicle = new Vehicle
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

        var repo = CreateVehicleRepository(new List<Vehicle> { existingVehicle });

        var newVehicle = new Vehicle
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

        repo.Invoking(r => r.Add(newVehicle)).Should().Throw<ArgumentException>()
            .WithMessage($"Машина с именем {existingVehicle.Name} уже существует");
    }
        
    [Fact]
    public void UpdateVehicle_VehicleWithSuchNameExists_ShouldThrowException()
    {
        var existingVehicle = new Vehicle
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
        var updatingVehicle = new Vehicle
        {
            Id = 2,
            Name = "Обновляемая",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            Description = "Описание 2",
            Make = "Ford",
            Model = "Fiesta",
            ManufacturingYear = 2020,
            RegistrationNumber = "В167АР 189",
            InventoryNumber = "1235"
        };

        var repo = CreateVehicleRepository(new List<Vehicle> { existingVehicle, updatingVehicle });

        updatingVehicle.Name = "Сущесвующая";

        repo.Invoking(r => r.Update(updatingVehicle)).Should().Throw<ArgumentException>()
            .WithMessage($"Машина с именем {existingVehicle.Name} уже существует");
    }

    private static VehicleRepository CreateVehicleRepository(ICollection<Vehicle> vehicleList)
    {
        IKeyValueProvider<Vehicle, int> keyValueProviderMock = new KeyValueProviderMock<Vehicle, int>(vehicleList);
        IQueryableProvider<Vehicle> vehicleQueryProviderMock = new QueryableProviderMock<Vehicle>(vehicleList);
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactoryMock = new BioTonDBContextUnitOfWorkFactoryMock();

        var repo = new VehicleRepository(keyValueProviderMock, vehicleQueryProviderMock, unitOfWorkFactoryMock);
        return repo;
    }

    private static IList<Vehicle> SampleVehicles => new List<Vehicle>
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
            InventoryNumber = "1234"
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
            InventoryNumber = "1235"
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
            InventoryNumber = "1236"
        }
    };
}