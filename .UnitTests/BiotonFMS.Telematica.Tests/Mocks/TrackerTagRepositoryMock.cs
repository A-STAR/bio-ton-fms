using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class TrackerTagRepositoryMock
{
    private static IEnumerable<TrackerTag> GetTrackerTags() =>
        new TrackerTag[]
        {
            new()
            {
                Id = 1,
                Name = "tagA"
            },
            new()
            {
                Id = 2,
                Name = "tagB"
            }
        };

    public static ITrackerTagRepository GetStub()
    {
        var repo = new Mock<ITrackerTagRepository>();
        repo.Setup(x => x.GetTags()).Returns(GetTrackerTags);
        return repo.Object;
    }
}
