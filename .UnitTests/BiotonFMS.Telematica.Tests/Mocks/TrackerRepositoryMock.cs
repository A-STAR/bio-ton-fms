using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Paging;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class TrackerRepositoryMock
{
    public const int NonExistentTrackerId = -1;
    public const int ExistentTrackerId = 1;
    private static PagedResult<Tracker> GetTrackers()
    {
        var sensors = SensorRepositoryMock.GetSensors().ToList();
        return new PagedResult<Tracker>
        {
            CurrentPage = 1, Results = new List<Tracker>
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
                }
            }
        };
    }

    public static ITrackerRepository GetStub()
    {
        var repo = new Mock<ITrackerRepository>();
        repo.Setup(x => x.GetTrackers(It.IsAny<TrackersFilter>(), false))
            .Returns(GetTrackers);
        repo.Setup(x => x[It.IsAny<int>()])
            .Returns((int i) => GetTrackers().Results.FirstOrDefault(x => x.Id == i));
        return repo.Object;
    }
}
