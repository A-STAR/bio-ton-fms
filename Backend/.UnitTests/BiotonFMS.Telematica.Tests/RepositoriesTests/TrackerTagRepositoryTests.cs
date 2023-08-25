using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using FluentAssertions;
using BioTonFMS.Infrastructure.EF;

namespace BiotonFMS.Telematica.Tests.RepositoriesTests;

public class TrackerTagRepositoryTests
{
    [Fact]
    public void GetTagsForTrackerType_TagsExists_ShouldReturnTags()
    {
        var repo = CreateTrackerTagRepository(SampleTags);

        var tags = repo.GetTagsForTrackerType(TrackerTypeEnum.GalileoSkyV50).ToList();

        tags.Count.Should().Be(2);
        tags.Should().ContainSingle(x => x.Id == 1);
        tags.Should().ContainSingle(x => x.Id == 3);
        tags.ForEach(x => x.ProtocolTags.Should().NotBeNullOrEmpty());
    }
    
    [Fact]
    public void GetTagsForTrackerType_NoSuchTagsExists_ShouldReturnEmptyList()
    {
        var repo = CreateTrackerTagRepository(SampleTags);

        var tags = repo.GetTagsForTrackerType(TrackerTypeEnum.Retranslator).ToList();

        tags.Should().NotBeNull();
        tags.Should().BeEmpty();
    }
    
    [Fact]
    public void GetTagForTrackerType_TagExists_ShouldReturnTag()
    {
        var repo = CreateTrackerTagRepository(SampleTags);

        var tag = repo.GetTagForTrackerType(TrackerTypeEnum.GalileoSkyV50, 0x3);

        tag.Should().NotBeNull();
        tag!.Id.Should().Be(1);
        tag.Name.Should().Be("imei");
        tag.ProtocolTags.Should().NotBeNullOrEmpty();
        Assert.Single(tag.ProtocolTags);
        tag.ProtocolTags.Single().Id.Should().Be(1);
    }
    
    [Fact]
    public void GetTagForTrackerType_NoSuchTagExists_ShouldReturnNull()
    {
        var repo = CreateTrackerTagRepository(SampleTags);

        var tags = repo.GetTagForTrackerType(TrackerTypeEnum.Retranslator, 0x300);

        tags.Should().BeNull();
    }
    
    private static ITrackerTagRepository CreateTrackerTagRepository(ICollection<TrackerTag> trackerTags)
    {
        IKeyValueProvider<TrackerTag, int> keyValueProviderMock = new KeyValueProviderMock<TrackerTag, int>(trackerTags);
        IQueryableProvider<TrackerTag> vehicleQueryProviderMock = new QueryableProviderMock<TrackerTag>(trackerTags);
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactoryMock = new BioTonDBContextUnitOfWorkFactoryMock();

        var repo = new TrackerTagRepository(keyValueProviderMock, vehicleQueryProviderMock, unitOfWorkFactoryMock);
        return repo;
    }
    
    private static IList<TrackerTag> SampleTags => new List<TrackerTag>
        {
            new ()
            {
                Id = 1,
                Name = "imei",
                DataType = TagDataTypeEnum.String,
                ProtocolTags = new List<ProtocolTag>
                {
                    new ()
                    {
                        Id = 1,
                        Size = 15,
                        TrackerType = TrackerTypeEnum.GalileoSkyV50,
                        ProtocolTagCode = 0x3
                    }
                }
            },
            new ()
            {
                Id = 2,
                Name = "device_id",
                DataType = TagDataTypeEnum.Integer,
                ProtocolTags = new List<ProtocolTag>
                {
                    new ()
                    {
                        Id = 2,
                        Size = 2,
                        TrackerType = TrackerTypeEnum.WialonIPS,
                        ProtocolTagCode = 0x6
                    }
                }
            },
            new ()
            {
                Id = 3,
                Name = "tracker_date",
                DataType = TagDataTypeEnum.DateTime,
                ProtocolTags = new List<ProtocolTag>
                {
                    new ()
                    {
                        Id = 3,
                        Size = 4,
                        TrackerType = TrackerTypeEnum.GalileoSkyV50,
                        ProtocolTagCode = 0x20
                    },
                    new ()
                    {
                        Id = 4,
                        Size = 4,
                        TrackerType = TrackerTypeEnum.WialonIPS,
                        ProtocolTagCode = 0x22
                    }
                }
            },
            new ()
            {
                Id = 4,
                Name = "rec_sn",
                DataType = TagDataTypeEnum.Integer,
                ProtocolTags = new List<ProtocolTag>()
            }
        };
}