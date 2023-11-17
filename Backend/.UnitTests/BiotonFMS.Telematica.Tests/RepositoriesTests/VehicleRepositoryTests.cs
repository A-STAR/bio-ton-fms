using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using FluentAssertions;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.RepositoriesTests;

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
                VehicleRepositoryMock.SampleVehicles.Where(x => x.Name == "Красная машина").ToList()
            },
            new object[]
            {
                "Type filter",
                new VehiclesFilter { Type = VehicleTypeEnum.Transport },
                VehicleRepositoryMock.SampleVehicles.Where(x => x.Type == VehicleTypeEnum.Transport).ToList()
            },
            new object[]
            {
                "Group filter",
                new VehiclesFilter { GroupId = 1 },
                VehicleRepositoryMock.SampleVehicles.Where(x => (x.VehicleGroupId ?? 0) == 1).ToList()
            },
            new object[]
            {
                "Subtype filter",
                new VehiclesFilter { SubType = VehicleSubTypeEnum.Car },
                VehicleRepositoryMock.SampleVehicles.Where(x => x.VehicleSubType == VehicleSubTypeEnum.Car).ToList()
            },
            new object[]
            {
                "Name asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Name, SortDirection = SortDirection.Ascending },
                VehicleRepositoryMock.SampleVehicles.OrderBy(x => x.Name).ToList(),
                true
            },
            new object[]
            {
                "Group asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Group, SortDirection = SortDirection.Ascending },
                VehicleRepositoryMock.SampleVehicles.OrderBy(x => x.VehicleGroup?.Name).ToList(),
                true
            },
            new object[]
            {
                "Type asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Type, SortDirection = SortDirection.Ascending },
                VehicleRepositoryMock.SampleVehicles.OrderBy(x => x.Type).ToList(),
                true
            },
            new object[]
            {
                "Fuel type asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.FuelType, SortDirection = SortDirection.Ascending },
                VehicleRepositoryMock.SampleVehicles.OrderBy(x => x.FuelType.Name).ToList(),
                true
            },
            new object[]
            {
                "Subtype asc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.SubType, SortDirection = SortDirection.Ascending },
                VehicleRepositoryMock.SampleVehicles.OrderBy(x => x.VehicleSubType).ToList(),
                true
            },
            new object[]
            {
                "Name desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Name, SortDirection = SortDirection.Descending },
                VehicleRepositoryMock.SampleVehicles.OrderByDescending(x => x.Name).ToList(),
                true
            },
            new object[]
            {
                "Group desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Group, SortDirection = SortDirection.Descending },
                VehicleRepositoryMock.SampleVehicles.OrderByDescending(x => x.VehicleGroup?.Name).ToList(),
                true
            },
            new object[]
            {
                "Type desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.Type, SortDirection = SortDirection.Descending },
                VehicleRepositoryMock.SampleVehicles.OrderByDescending(x => x.Type).ToList(),
                true
            },
            new object[]
            {
                "Fuel type desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.FuelType, SortDirection = SortDirection.Descending },
                VehicleRepositoryMock.SampleVehicles.OrderByDescending(x => x.FuelType.Name).ToList(),
                true
            },
            new object[]
            {
                "Subtype desc sort",
                new VehiclesFilter { SortBy = VehicleSortBy.SubType, SortDirection = SortDirection.Descending },
                VehicleRepositoryMock.SampleVehicles.OrderByDescending(x => x.VehicleSubType).ToList(),
                true
            }
        };

    [Theory, MemberData(nameof(Data))]
    public void GetVehicles_WithFilters_ShouldFilter(string testName, VehiclesFilter filter,
        List<Vehicle> expected, bool considerOrder = false)
    {
        _testOutputHelper.WriteLine(testName);

        var results = VehicleRepositoryMock.GetStub().GetVehicles(filter).Results;

        Assert.Equal(expected.Count, results.Count);

        if (considerOrder)
            results.Should().Equal(expected);
        else
            results.Should().BeEquivalentTo(expected);
    }

    #endregion

    #region Find

    public static IEnumerable<object[]> CriterionData =>
        new List<object[]>
        {
            new object[]
            {
                "красн",
                VehicleRepositoryMock.SampleVehicles.Where(x => x.Name == "Красная машина").ToArray()
            },
            new object[]
            {
                "ая",
                VehicleRepositoryMock.SampleVehicles.Where(x => x.Name is "Красная машина" or "Желтая машина").ToArray()
            },
            new object[]
            {
                "512512",
                VehicleRepositoryMock.SampleVehicles.Where(x => x.Tracker?.Imei == "512512").ToArray()
            },
            new object[]
            {
                "512",
                Array.Empty<Vehicle>()
            },
            new object[]
            {
                "15",
                VehicleRepositoryMock.SampleVehicles.Where(x => x.Tracker?.ExternalId == 15).ToArray()
            },
            new object[]
            {
                "",
                VehicleRepositoryMock.SampleVehicles.Where(x => x.TrackerId != null).ToArray()
            },
            new object[]
            {
                "такой строки нет",
                Array.Empty<Vehicle>()
            },
            new object[]
            {
                null!,
                VehicleRepositoryMock.SampleVehicles.Where(x => x.TrackerId != null).ToArray()
            }
        };

    [Theory, MemberData(nameof(CriterionData))]
    public void FindVehicle_WithCriterion_ShouldFilter(
        string? criterion, Vehicle[] expected)
    {
        _testOutputHelper.WriteLine("Criterion = \"" + criterion + "\"");

        var results = VehicleRepositoryMock.GetStub().FindVehicles(criterion);

        Assert.Equal(expected.Length, results.Length);

        results.Should().BeEquivalentTo(expected);
    }

    #endregion

    [Fact]
    public void AddVehicle_VehicleWithSuchNameExists_ShouldThrowException()
    {
        var existingVehicle = new Vehicle
        {
            Id = 1,
            Name = "Существующая",
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

        var repo = VehicleRepositoryMock.GetStub(new List<Vehicle> { existingVehicle });

        var newVehicle = new Vehicle
        {
            Name = "Существующая",
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
            Name = "Существующая",
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

        var repo = VehicleRepositoryMock.GetStub(new List<Vehicle> { existingVehicle, updatingVehicle });

        updatingVehicle.Name = "Существующая";

        repo.Invoking(r => r.Update(updatingVehicle)).Should().Throw<ArgumentException>()
            .WithMessage($"Машина с именем {existingVehicle.Name} уже существует");
    }

    [Fact]
    public void AddVehicle_VehicleWithSuchTrackerExists_ShouldThrowException()
    {
        var existingVehicle = new Vehicle
        {
            Id = 1,
            Name = "Машина",
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
            TrackerId = 1
        };

        var existingTracker = new Tracker
        {
            Id = 1,
            Name = "Трекер",
            Description = "Описание 1",
            Vehicle = existingVehicle,
            Imei = "1234567889",
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            ExternalId = 1,
            StartDate = DateTime.UtcNow
        };

        existingVehicle.Tracker = existingTracker;

        var repo = VehicleRepositoryMock.GetStub(new List<Vehicle> { existingVehicle });

        var newVehicle = new Vehicle
        {
            Name = "Новая машина",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            Description = "Описание 1",
            Make = "Ford",
            Model = "Mondeo",
            ManufacturingYear = 2019,
            RegistrationNumber = "В165АР 199",
            InventoryNumber = "1235",
            TrackerId = 1,
            Tracker = existingTracker
        };

        repo.Invoking(r => r.Add(newVehicle)).Should().Throw<ArgumentException>()
            .WithMessage($"Трекер {existingTracker.Name} уже используется для машины {existingVehicle.Name}");
    }

    [Fact]
    public void UpdateVehicle_VehicleWithSuchTrackerExists_ShouldThrowException()
    {
        var existingVehicle = new Vehicle
        {
            Id = 1,
            Name = "Существующая",
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

        var existingTracker = new Tracker
        {
            Id = 1,
            Name = "Трекер",
            Description = "Описание 1",
            Vehicle = existingVehicle,
            Imei = "1234567889",
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            ExternalId = 1,
            StartDate = DateTime.UtcNow
        };

        existingVehicle.Tracker = existingTracker;

        var repo = VehicleRepositoryMock.GetStub(new List<Vehicle> { existingVehicle, updatingVehicle });

        updatingVehicle.TrackerId = 1;
        updatingVehicle.Tracker = existingTracker;

        repo.Invoking(r => r.Update(updatingVehicle)).Should().Throw<ArgumentException>()
            .WithMessage($"Трекер {existingTracker.Name} уже используется для машины {existingVehicle.Name}");
    }

    public static IEnumerable<object[]> VehicleGetTrackerData =>
        new List<object[]>
        {
            // Машина с сообщениями и без настроенных датчиков
            new object[]
            {
                1,
                new Tracker
                {
                    Id = 1,
                    Imei = "123",
                    ExternalId = 2552
                }
            },
            new object[]
            {
                2,
                null!
            }
        };

    [Theory, MemberData(nameof(VehicleGetTrackerData))]
    public void GetTracker_ShouldReturnCorrectTracker(int vehicleId, Tracker? expected)
    {
        var repository = VehicleRepositoryMock.GetStub(VehiclesForGetTracker);
        var tracker = repository.GetTracker(vehicleId);
        if (expected != null)
        {
            tracker.Should().BeEquivalentTo(expected);
        }
    }

    public static IList<Vehicle> VehiclesForGetTracker => new List<Vehicle>
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
            TrackerId = 2,
            Tracker = new Tracker
            {
                Id = 2,
                Imei = "128128",
                ExternalId = 15
            }
        }
    };
}