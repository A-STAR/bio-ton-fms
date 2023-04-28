using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class TrackerTagRepositoryMock
{
    public static ITrackerTagRepository GetStub()
    {
        var repo = new Mock<ITrackerTagRepository>();
        repo.Setup(x => x.GetTags()).Returns(TagsSeed.TrackerTags);
        return repo.Object;
    }
}
