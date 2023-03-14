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
    private static PagedResult<Tracker> GetTrackers() =>
        new()
        {
            CurrentPage = 1,
            Results = new List<Tracker>
            {
                new()
                {
                    Id = 1
                },
                new()
                {
                    Id = 2
                }
            }
        };

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