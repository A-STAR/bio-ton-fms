using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.ProtocolTags;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class ProtocolTagRepositoryMock
{
    public static IProtocolTagRepository GetStub()
    {
        var tagStub = new Mock<IProtocolTagRepository>();

        var trackerTags = TagsSeed.TrackerTags
            .ToDictionary(x => x.Id);

        var protocolTags = TagsSeed.ProtocolTags
            .Where(x => x.TrackerType == TrackerTypeEnum.GalileoSkyV50)
            .Select(x =>
            {
                if (x.TagId.HasValue && trackerTags.TryGetValue(x.TagId.Value, out var tag))
                    x.Tag = tag;
                return x;
            })
            .ToArray();

        tagStub.Setup(x => x.GetTagsForTrackerType(TrackerTypeEnum.GalileoSkyV50))
            .Returns(protocolTags);

        return tagStub.Object;
    }
}