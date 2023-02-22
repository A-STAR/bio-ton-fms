using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.RepoTests;

public class TrackerRepositoryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TrackerRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #region Filters

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[]
            {
                "ExternalId filter",
                new TrackersFilter { ExternalId = 111 },
                SampleTrackers.Where(x => x.ExternalId == 111).ToList()
            },
            new object[]
            {
                "Tracker type filter",
                new TrackersFilter { Type = TrackerTypeEnum.GalileoSkyV50 },
                SampleTrackers.Where(x => x.TrackerType == TrackerTypeEnum.GalileoSkyV50).ToList()
            },
            new object[]
            {
                "SimNumber filter",
                new TrackersFilter { SimNumber = "905518101010" },
                SampleTrackers.Where(x => x.SimNumber == "905518101010").ToList()
            },
            new object[]
            {
                "Name asc sort",
                new TrackersFilter { SortBy = TrackerSortBy.Name, SortDirection = SortDirection.Ascending },
                SampleTrackers.OrderBy(x => x.Name).ToList(),
                true
            },
            new object[]
            {
                "Tracker type asc sort",
                new TrackersFilter { SortBy = TrackerSortBy.Type, SortDirection = SortDirection.Ascending },
                SampleTrackers.OrderBy(x => x.TrackerType).ToList(),
                true
            },
            new object[]
            {
                "External id asc sort",
                new TrackersFilter { SortBy = TrackerSortBy.ExternalId, SortDirection = SortDirection.Ascending },
                SampleTrackers.OrderBy(x => x.ExternalId).ToList(),
                true
            },
            new object[]
            {
                "Vehicle asc sort",
                new TrackersFilter { SortBy = TrackerSortBy.Vehicle, SortDirection = SortDirection.Ascending },
                SampleTrackers.OrderBy(x => x.Vehicle?.Name).ToList(),
                true
            },
            new object[]
            {
                "SimNumber asc sort",
                new TrackersFilter { SortBy = TrackerSortBy.SimNumber, SortDirection = SortDirection.Ascending },
                SampleTrackers.OrderBy(x => x.SimNumber).ToList(),
                true
            },
            new object[]
            {
                "StartDate asc sort",
                new TrackersFilter { SortBy = TrackerSortBy.StartDate, SortDirection = SortDirection.Ascending },
                SampleTrackers.OrderBy(x => x.StartDate).ToList(),
                true
            },
            new object[]
            {
                "Name desc sort",
                new TrackersFilter { SortBy = TrackerSortBy.Name, SortDirection = SortDirection.Descending },
                SampleTrackers.OrderByDescending(x => x.Name).ToList(),
                true
            },
            new object[]
            {
                "Tracker type desc sort",
                new TrackersFilter { SortBy = TrackerSortBy.Type, SortDirection = SortDirection.Descending },
                SampleTrackers.OrderByDescending(x => x.TrackerType).ToList(),
                true
            },
            new object[]
            {
                "ExternalId desc sort",
                new TrackersFilter { SortBy = TrackerSortBy.ExternalId, SortDirection = SortDirection.Descending },
                SampleTrackers.OrderByDescending(x => x.ExternalId).ToList(),
                true
            },
            new object[]
            {
                "Vehicle desc sort",
                new TrackersFilter { SortBy = TrackerSortBy.Vehicle, SortDirection = SortDirection.Descending },
                SampleTrackers.OrderByDescending(x => x.Vehicle?.Name).ToList(),
                true
            },
            new object[]
            {
                "SimNumber desc sort",
                new TrackersFilter { SortBy = TrackerSortBy.SimNumber, SortDirection = SortDirection.Descending },
                SampleTrackers.OrderByDescending(x => x.SimNumber).ToList(),
                true
            },
            new object[]
            {
                "StartDate desc sort",
                new TrackersFilter { SortBy = TrackerSortBy.StartDate, SortDirection = SortDirection.Descending },
                SampleTrackers.OrderByDescending(x => x.StartDate).ToList(),
                true
            }
        };

    [Theory, MemberData(nameof(Data))]
    public void GetVehicles_WithFilters_ShouldFilter(string testName, TrackersFilter filter,
        List<Tracker> expected, bool considerOrder = false)
    {
        _testOutputHelper.WriteLine(testName);

        var results = CreateTrackerRepository(SampleTrackers, SampleVehicles).GetTrackers(filter).Results;

        Assert.Equal(results.Count, expected.Count);

        if (considerOrder)
            results.Should().Equal(expected);
        else
            results.Should().BeEquivalentTo(expected);
    }

    #endregion

    [Fact]
    public void AddTracker_TrackerWithSuchExternalIdExists_ShouldThrowException()
    {
        var existing = new Tracker
        {
            Id = 1,
            Name = "Сущесвующий",
            Description = "Описание 1",
            Imei = "1234",
            ExternalId = 420,
            SimNumber = "88005553535",
            StartDate = DateTime.UtcNow
        };

        var repo = CreateTrackerRepository(new List<Tracker> { existing }, new List<Vehicle>());

        var tracker = new Tracker
        {
            Name = "Новый",
            Description = "Описание 2",
            Imei = "4321",
            ExternalId = 420,
            SimNumber = "+78005553535",
            StartDate = DateTime.UtcNow
        };

        repo.Invoking(r => r.Add(tracker)).Should().Throw<ArgumentException>()
            .WithMessage($"Трекер с внешним идентификатором {tracker.ExternalId} уже существует");
    }

    [Fact]
    public void UpdateTracker_TrackerWithSuchExternalIdExists_ShouldThrowException()
    {
        var existing = new Tracker
        {
            Id = 1,
            Name = "Сущесвующий",
            Description = "Описание 1",
            Imei = "1234",
            ExternalId = 420,
            SimNumber = "88005553535",
            StartDate = DateTime.UtcNow
        };

        var updating = new Tracker
        {
            Id = 2,
            Name = "Обновляемый",
            Description = "Описание 2",
            Imei = "1234",
            ExternalId = 240,
            SimNumber = "88005553535",
            StartDate = DateTime.UtcNow
        };

        var repo = CreateTrackerRepository(new List<Tracker> { existing, updating }, new List<Vehicle>());

        updating.ExternalId = 420;

        repo.Invoking(r => r.Update(updating)).Should().Throw<ArgumentException>()
            .WithMessage($"Трекер с внешним идентификатором {existing.ExternalId} уже существует");
    }

    [Fact]
    public void RemoveTracker_TrackerRelatedToVehicle_ShouldThrowException()
    {
        var tracker = new Tracker
        {
            Id = 1,
            Name = "Трекер",
            Description = "Описание 1",
            Imei = "1234",
            ExternalId = 420,
            SimNumber = "88005553535",
            StartDate = DateTime.UtcNow
        };

        var vehicle = new Vehicle
        {
            Id = 1,
            Name = "Машина",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            Description = "Описание 2",
            Make = "Ford",
            Model = "Fiesta",
            ManufacturingYear = 2020,
            RegistrationNumber = "В167АР 189",
            InventoryNumber = "1235",
            Tracker = tracker,
            TrackerId = 1
        };

        tracker.Vehicle = vehicle;

        var repo = CreateTrackerRepository(
            new List<Tracker> { tracker },
            new List<Vehicle> { vehicle });

        repo.Invoking(r => r.Remove(tracker)).Should().Throw<ArgumentException>()
            .WithMessage($"Нельзя удалить трекер привязанный к машине (название - '{vehicle.Name}', " +
                         $"регистрационный номер - {vehicle.RegistrationNumber})");
    }

    private static TrackerRepository CreateTrackerRepository(ICollection<Tracker> trackers,
        ICollection<Vehicle> vehicles)
    {
        IKeyValueProvider<Tracker, int> keyValueProviderMock = new KeyValueProviderMock<Tracker, int>(trackers);
        IQueryableProvider<Tracker> trackerQueryProviderMock = new QueryableProviderMock<Tracker>(trackers);
        IQueryableProvider<Vehicle> vehicleQueryProviderMock = new QueryableProviderMock<Vehicle>(vehicles);
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactoryMock = new BioTonDBContextUnitOfWorkFactoryMock();
        var loggerMock = new Mock<ILogger<TrackerRepository>>().Object;

        var repo = new TrackerRepository(loggerMock, vehicleQueryProviderMock, keyValueProviderMock,
            trackerQueryProviderMock, unitOfWorkFactoryMock);
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
            InventoryNumber = "1236",
        }
    };

    private static IList<Tracker> SampleTrackers => new List<Tracker>
    {
        new()
        {
            Id = 1,
            Name = "трекер GalileoSky",
            Description = "Описание 1",
            Imei = "12341",
            ExternalId = 111,
            StartDate = DateTime.MinValue,
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            SimNumber = "905518101010",
            Vehicle = SampleVehicles[0]
        },
        new()
        {
            Id = 2,
            Name = "трекер Retranslator",
            Description = "Описание 2",
            Imei = "12342",
            ExternalId = 222,
            StartDate = DateTime.UnixEpoch,
            TrackerType = TrackerTypeEnum.Retranslator,
            SimNumber = "905518101020",
            Vehicle = SampleVehicles[1]
        },
        new()
        {
            Id = 3,
            Name = "трекер WialonIPS",
            Description = "Описание 3",
            Imei = "12343",
            ExternalId = 333,
            StartDate = DateTime.MaxValue,
            TrackerType = TrackerTypeEnum.WialonIPS,
            SimNumber = "905518101030",
            Vehicle = SampleVehicles[2]
        }
    };
}