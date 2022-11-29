using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Paging;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class TrackerMock
{
    public static PagedResult<Tracker> GetTrackers() =>
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
        repo.Setup(x => x.GetTrackers(It.IsAny<TrackersFilter>()))
            .Returns(GetTrackers);
        return repo.Object;
    }
}