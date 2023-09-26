using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Moq;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Controllers;
using Microsoft.Extensions.Logging;

namespace BiotonFMS.Telematica.Tests.Mocks.Repositories;

public static class TrackerRepositoryMock
{
    public const int NonExistentTrackerId = -1;
    public const int ExistentTrackerId = 1;
    private static ICollection<Tracker> GetTrackers()
    {
        var sensors = SensorRepositoryMock.GetSensors().ToList();
        return new List<Tracker>
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
                Sensors = sensors.Where(s => s.TrackerId == 1).ToList()
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
                Sensors = sensors.Where(s => s.TrackerId == 2).ToList()
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
                Sensors = sensors.Where(s => s.TrackerId == 3).ToList()
            },
            new()
            {
                Id = 121,
                Name = "трекер WialonIPS",
                Description = "Описание 121",
                Imei = "123435",
                ExternalId = 444,
                StartDate = DateTime.MaxValue,
                TrackerType = TrackerTypeEnum.WialonIPS,
                SimNumber = "905518101030",
                Sensors = sensors.Where(s => s.TrackerId == 3).ToList()
            }
        };
    }

    public static ITrackerRepository GetStub(ICollection<Tracker>? trackers = null)
    {
        if (trackers == null)
        {
            trackers = GetTrackers();
        }

        var logger = new Mock<ILogger<TrackerRepository>>().Object;
        var vehicleQueryProviderMock = new QueryableProviderMock<Vehicle>(VehicleRepositoryMock.SampleVehicles);
        var keyValueProviderMock = new KeyValueProviderMock<Tracker, int>(trackers);
        var trackerQueryProviderMock = new QueryableProviderMock<Tracker>(trackers);
        var unitOfWorkFactoryMock = new BioTonDBContextUnitOfWorkFactoryMock();

        var repository = new TrackerRepository(logger, vehicleQueryProviderMock, keyValueProviderMock, trackerQueryProviderMock, unitOfWorkFactoryMock);
        return repository;
    }
}
