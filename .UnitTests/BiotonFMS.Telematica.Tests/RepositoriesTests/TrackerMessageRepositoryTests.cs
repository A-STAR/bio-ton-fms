using System.Collections;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.Paging;
using BiotonFMS.Telematica.Tests.Mocks;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.RepositoriesTests;

public class TrackerMessageRepositoryTests
{
    #region ExistsByUid
    [Fact]
    public void ExistsByUid_MessageExists_ShouldReturnTrue()
    {
        var repo = GetRepo(Messages);
        bool result = repo.ExistsByUid(Guid.Parse("829C3996-DB42-4777-A4D5-BB6D8A9E3B79"));

        result.Should().BeTrue();
    }

    [Fact]
    public void ExistsByUid_MessageNotExists_ShouldReturnFalse()
    {
        var repo = GetRepo(Messages);
        bool result = repo.ExistsByUid(Guid.Parse("829C3996-DB42-4777-A4D5-BB6D8A9E3B89"));

        result.Should().BeFalse();
    }
    #endregion

    #region GetParameters
    [Theory]
    [InlineData(2552)]
    public void GetParameters_ShouldReturnParameters(int externalId)
    {
        var repo = GetRepo(Messages);
        var parameters = repo.GetParameters(externalId);
        parameters.Count.Should().Be(3);
        parameters.First(x => x.ParamName == "hdop").LastValueDecimal.Should().Be(6);
        parameters.First(x => x.ParamName == "rec_sn").LastValueDecimal.Should().Be(12345);
        parameters.First(x => x.ParamName == "rs485_1").LastValueDecimal.Should().Be(2134);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void GetParameters_NoMessages_ShouldReturnEmptyCollection(int externalId)
    {
        var repo = GetRepo(Messages);
        var parameters = repo.GetParameters(externalId);

        parameters.Should().Equal(Enumerable.Empty<TrackerParameter>());
    }

    [Fact]
    public void GetParameters_ShouldParseTagsCorrectly()
    {
        var messages = new[]
        {
            new TrackerMessage
            {
                Id = 0,
                ExternalTrackerId = 2552,
                Imei = "123",
                Tags = new List<MessageTag>
                {
                    new MessageTagBits
                    {
                        TrackerTagId = 15,
                        Value = new BitArray(5, true),
                        TagType = TagDataTypeEnum.Bits
                    },
                    new MessageTagByte
                    {
                        TrackerTagId = 10,
                        Value = 5,
                        TagType = TagDataTypeEnum.Byte
                    },
                    new MessageTagInteger
                    {
                        TrackerTagId = 111,
                        Value = 40,
                        TagType = TagDataTypeEnum.Integer
                    },
                    new MessageTagDouble
                    {
                        TrackerTagId = 121,
                        Value = 13.32,
                        TagType = TagDataTypeEnum.Double
                    },
                    new MessageTagString
                    {
                        TrackerTagId = 3,
                        Value = "123",
                        TagType = TagDataTypeEnum.String
                    },
                    new MessageTagDateTime
                    {
                        TrackerTagId = 6,
                        Value = DateTime.UnixEpoch,
                        TagType = TagDataTypeEnum.DateTime
                    }
                }
            }
        };
        var repo = GetRepo(messages);
        var parameters = repo.GetParameters(2552);

        parameters.Count.Should().Be(6);
        parameters.First(x => x.ParamName == "out").LastValueString.Should().Be("11111");
        parameters.First(x => x.ParamName == "hdop").LastValueDecimal.Should().Be(5);
        parameters.First(x => x.ParamName == "coolant_temperature").LastValueDecimal.Should().Be(40);
        parameters.First(x => x.ParamName == "direction").LastValueDecimal.Should().Be(13.32);
        parameters.First(x => x.ParamName == "imei").LastValueString.Should().Be("123");
        parameters.First(x => x.ParamName == "tracker_date").LastValueDateTime.Should().Be(DateTime.UnixEpoch);
    }
    #endregion

    #region GetStandardParameters
    [Theory]
    [InlineData(2552)]
    public void GetStandardParameters_ShouldReturnStandardParameters(int externalId)
    {
        var repo = GetRepo(Messages);
        var parameters = repo.GetStandardParameters(externalId);
        parameters.Speed.Should().Be(12.1);
        parameters.Alt.Should().Be(97.0);
        parameters.Long.Should().Be(52.556861);
        parameters.Lat.Should().Be(49.432023);
        parameters.Time.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void GetStandardParameters_NoMessages_ShouldReturnModelWithNulls(int externalId)
    {
        var repo = GetRepo(Messages);
        var parameters = repo.GetStandardParameters(externalId);
        parameters.Speed.Should().BeNull();
        parameters.Alt.Should().BeNull();
        parameters.Long.Should().BeNull();
        parameters.Lat.Should().BeNull();
        parameters.Time.Should().BeNull();
    }

    [Theory]
    [InlineData(2552)]
    public void GetStandardParameters_MessagesWithNullParameters_ShouldReturnModelWithNulls(int externalId)
    {
        var messages = new TrackerMessage[]
        {
            new()
            {
                Id = 0,
                ExternalTrackerId = 2552,
                Imei = "123",
                ServerDateTime = DateTime.UtcNow
            },
            new()
            {
                Id = 1,
                ExternalTrackerId = 2552,
                Imei = "123",
                ServerDateTime = DateTime.UtcNow + TimeSpan.FromSeconds(2)
            }
        };
        var repo = GetRepo(messages);
        var parameters = repo.GetStandardParameters(externalId);
        parameters.Speed.Should().BeNull();
        parameters.Alt.Should().BeNull();
        parameters.Long.Should().BeNull();
        parameters.Lat.Should().BeNull();
        parameters.Time.Should().NotBeNull();
    }
    #endregion

    #region GetParametersHistory
    [Theory]
    [InlineData(2552)]
    public void GetParametersHistory_ShouldReturnParametersHistory(int externalId)
    {
        var repo = GetRepo(Messages);
        var filter = new ParametersHistoryFilter
        {
            ExternalId = externalId,
            PageNum = 1,
            PageSize = 10
        };
        var history = repo.GetParametersHistory(filter);

        history.Results.Count.Should().Be(2);
        history.Should().BeAssignableTo<PagedResult<ParametersHistoryRecord>>();
        history.Results.First().Time.Should().BeAfter(history.Results.Last().Time);
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[]
            {
                new MessageTag[]
                {
                    new MessageTagString
                    {
                        TrackerTagId = 3,
                        Value = "123"
                    }
                },
                "imei=123,"
            },
            new object[]
            {
                new MessageTag[]
                {
                    new MessageTagBits
                    {
                        TrackerTagId = 16,
                        Value = new BitArray(4, true)
                    }
                },
                "in=1111,"
            },
            new object[]
            {
                new MessageTag[]
                {
                    new MessageTagInteger
                    {
                        TrackerTagId = 111,
                        Value = 40
                    }
                },
                "coolant_temperature=40,"
            },
            new object[]
            {
                new MessageTag[]
                {
                    new MessageTagDouble
                    {
                        TrackerTagId = 121,
                        Value = 14.1
                    }
                },
                "direction=14.1,"
            },
            new object[]
            {
                new MessageTag[]
                {
                    new MessageTagByte
                    {
                        TrackerTagId = 10,
                        Value = 5
                    }
                },
                "hdop=5,"
            },
            new object[]
            {
                new MessageTag[]
                {
                    new MessageTagDateTime
                    {
                        TrackerTagId = 6,
                        Value = DateTime.UnixEpoch
                    }
                },
                "tracker_date=01/01/1970 00:00:00,",
            },
            new object[]
            {
                new MessageTag[]
                {
                    new MessageTagDateTime
                    {
                        TrackerTagId = 6,
                        Value = DateTime.UnixEpoch
                    },
                    new MessageTagByte
                    {
                        TrackerTagId = 10,
                        Value = 5
                    },
                    new MessageTagDouble
                    {
                        TrackerTagId = 121,
                        Value = 14.1
                    },
                    new MessageTagInteger
                    {
                        TrackerTagId = 111,
                        Value = 40
                    },
                    new MessageTagBits
                    {
                        TrackerTagId = 16,
                        Value = new BitArray(4, true)
                    },
                    new MessageTagString
                    {
                        TrackerTagId = 3,
                        Value = "123"
                    }
                },
                "tracker_date=01/01/1970 00:00:00,hdop=5,direction=14.1,coolant_temperature=40,in=1111,imei=123,"
            }
        };

    [Theory, MemberData(nameof(Data))]
    public void GetParametersHistory_ShouldAggregateTagsCorrectly(MessageTag[] tags, string aggregated)
    {
        var messages = new TrackerMessage[]
        {
            new()
            {
                Id = 0,
                ExternalTrackerId = 2552,
                Imei = "123",
                Tags = tags
            }
        };
        var repo = GetRepo(messages);
        var filter = new ParametersHistoryFilter
        {
            ExternalId = 2552,
            PageNum = 1,
            PageSize = 10
        };
        var history = repo.GetParametersHistory(filter);

        history.Results.First().Parameters.Should().Be(aggregated);
    }
    #endregion

    #region GetLastMessageFor
    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void GetLastMessageFor_EmptyOrNonexistentParams_ShouldReturnNull(int externalId)
    {
        var repo = GetRepo(Messages);
        var last = repo.GetLastMessagesFor(new[] { externalId });

        last.Should().NotBeNull();
        last.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(2552)]
    public void GetLastMessageFor_ExistentParams_ShouldReturnLastMessage(int externalId)
    {
        var repo = GetRepo(Messages);
        var last = repo.GetLastMessagesFor(new[] { externalId });

        last.Should().NotBeNull();
        last.Count.Should().Be(1);
        last[externalId]!.Imei.Should().Be("123");
        last[externalId].ExternalTrackerId.Should().Be(2552);
        last[externalId].Speed.Should().Be(12.1);
        last[externalId].PackageUID.Should().Be(Guid.Parse("829C3996-DB42-4777-A4D5-BB6D8A9E3B79"));
    }
    #endregion

    private static ITrackerMessageRepository GetRepo(ICollection<TrackerMessage> messages)
    {
        var kvp = new KeyValueProviderMock<TrackerMessage, int>(messages);
        var qp = new QueryableProviderMock<TrackerMessage>(messages);
        var uow = new MessagesDBContextUnitOfWorkFactoryMock();
        var repo = new TrackerMessageRepository(kvp, qp, uow, TrackerTagRepositoryMock.GetStub());
        return repo;
    }

    private TrackerMessage[] Messages => new TrackerMessage[]
    {
        new()
        {
            Id = 0,
            ExternalTrackerId = 2552,
            Imei = "123",
            ServerDateTime = DateTime.UtcNow,
            TrackerDateTime = DateTime.UtcNow,
            Latitude = 49.432023,
            Longitude = 52.556861,
            SatNumber = 12,
            CoordCorrectness = 0,
            Altitude = 97.0,
            Direction = 2.8,
            FuelLevel = 100,
            CoolantTemperature = 45,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("F28AC4A2-5DD0-49DC-B8B5-3B161C39546A"),
            Tags = new List<MessageTag>
            {
                new MessageTagInteger
                {
                    Value = 1234,
                    TrackerTagId = 5,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagByte
                {
                    Value = 6,
                    TrackerTagId = 10,
                    TagType = TagDataTypeEnum.Byte
                },
                new MessageTagBits
                {
                    Value = new BitArray(new byte[] { 215 }),
                    TrackerTagId = 15,
                    TagType = TagDataTypeEnum.Bits
                }
            }
        },
        new()
        {
            Id = 1,
            ExternalTrackerId = 2552,
            Imei = "123",
            ServerDateTime = DateTime.UtcNow + TimeSpan.FromSeconds(2),
            TrackerDateTime = DateTime.UtcNow + TimeSpan.FromSeconds(2),
            Latitude = 49.432023,
            Longitude = 52.556861,
            SatNumber = 12,
            CoordCorrectness = 0,
            Altitude = 97.0,
            Speed = 12.1,
            Direction = 2.8,
            FuelLevel = 100,
            CoolantTemperature = 45,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("829C3996-DB42-4777-A4D5-BB6D8A9E3B79"),
            Tags = new List<MessageTag>
            {
                new MessageTagInteger
                {
                    Value = 12345,
                    TrackerTagId = 5,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagByte
                {
                    Value = 6,
                    TrackerTagId = 10,
                    TagType = TagDataTypeEnum.Byte
                },
                new MessageTagInteger
                {
                    Value = 2134,
                    TrackerTagId = 24,
                    TagType = TagDataTypeEnum.Integer
                }
            }
        }
    };
}
