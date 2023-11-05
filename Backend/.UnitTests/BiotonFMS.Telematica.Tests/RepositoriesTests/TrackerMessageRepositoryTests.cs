using System.Collections;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.Paging;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using FluentAssertions;
using Xunit.Abstractions;
using BioTonFMS.Common.Testable;

namespace BiotonFMS.Telematica.Tests.RepositoriesTests;

/// <summary>
/// Тесты для <see cref="TrackerMessageRepository"/>
/// </summary>
public class TrackerMessageRepositoryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TrackerMessageRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    #region ExistsByUid
    [Fact]
    public void ExistsByUid_MessageExists_ShouldReturnTrue()
    {
        var repo = TrackerMessageRepositoryMock.GetStub();
        bool result = repo.ExistsByUid(Guid.Parse("829C3996-DB42-4777-A4D5-BB6D8A9E3B79"));

        result.Should().BeTrue();
    }

    [Fact]
    public void ExistsByUid_MessageNotExists_ShouldReturnFalse()
    {
        var repo = TrackerMessageRepositoryMock.GetStub();
        bool result = repo.ExistsByUid(Guid.Parse("829C3996-DB42-4777-A4D5-BB6D8A9E3B89"));

        result.Should().BeFalse();
    }
    #endregion

    #region GetParameters
    [Theory]
    [InlineData(2552)]
    public void GetParameters_ShouldReturnParameters(int externalId)
    {
        var repo = TrackerMessageRepositoryMock.GetStub();
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
        var repo = TrackerMessageRepositoryMock.GetStub();
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
        var repo = TrackerMessageRepositoryMock.GetStub(messages);
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
        var repo = TrackerMessageRepositoryMock.GetStub();
        var parameters = repo.GetStandardParameters(externalId);
        parameters.Speed.Should().Be(12.1);
        parameters.Alt.Should().Be(97.0);
        parameters.Long.Should().Be(54.556861);
        parameters.Lat.Should().Be(42.432023);
        parameters.Time.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void GetStandardParameters_NoMessages_ShouldReturnModelWithNulls(int externalId)
    {
        var repo = TrackerMessageRepositoryMock.GetStub();
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
        var repo = TrackerMessageRepositoryMock.GetStub(messages);
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
        var repo = TrackerMessageRepositoryMock.GetStub();
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
        var repo = TrackerMessageRepositoryMock.GetStub(messages);
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
        var repo = TrackerMessageRepositoryMock.GetStub();
        var last = repo.GetLastMessageFor(externalId);

        last.Should().BeNull();
    }

    [Theory]
    [InlineData(2552)]
    public void GetLastMessageFor_ExistentParams_ShouldReturnLastMessage(int externalId)
    {
        var repo = TrackerMessageRepositoryMock.GetStub();
        var last = repo.GetLastMessageFor(externalId);

        last.Should().NotBeNull();
        last!.Imei.Should().Be("123");
        last.ExternalTrackerId.Should().Be(2552);
        last.Speed.Should().Be(12.1);
        last.PackageUID.Should().Be(Guid.Parse("829C3996-DB42-4777-A4D5-BB6D8A9E3B79"));
    }
    #endregion

    #region GetVehicleStates
    
    public static IEnumerable<object[]> StatesData =>
        new List<object[]>
        {
            new object[]
            {
                new [] {2552},
                new Dictionary<int, VehicleStatus>
                {
                    {2552, new VehicleStatus
                    {
                        TrackerExternalId = 2552,
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Moving,
                        NumberOfSatellites = 14,
                        LastMessageTime = DateTime.UtcNow
                    }}
                }
            },
            new object[]
            {
                new int[] {},
                new Dictionary<int, VehicleStatus>()
            },
            new object[]
            {
                new [] {2552, 1024, 128, 190384},
                new Dictionary<int, VehicleStatus>
                {
                    {2552, new VehicleStatus
                    {
                        TrackerExternalId = 2552,
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Moving,
                        NumberOfSatellites = 14,
                        LastMessageTime = DateTime.UtcNow
                    }},
                    {1024, new VehicleStatus
                    {
                        TrackerExternalId = 1024,
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.NoData,
                        NumberOfSatellites = 1,
                        LastMessageTime = DateTime.UtcNow
                    }},
                    {128, new VehicleStatus
                    {
                        TrackerExternalId = 128,
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Stopped,
                        NumberOfSatellites = 19
                    }}
                }
            },
        };
    
    [Theory, MemberData(nameof(StatesData))]
    public void GetVehicleStates_ShouldReturnRightSates(int[] externalIds,
        IDictionary<int, VehicleStatus> expected)
    {
        _testOutputHelper.WriteLine("External IDs: " + string.Join(" ", externalIds));

        var results = TrackerMessageRepositoryMock.GetStub().GetVehicleStates(externalIds, 60);

        Assert.Equal(expected.Count, results.Count);

        results.Keys.Should().BeEquivalentTo(expected.Keys);

        foreach (var r in results)
        {
            expected[r.Key].ConnectionStatus.Should().Be(r.Value.ConnectionStatus);
            expected[r.Key].MovementStatus.Should().Be(r.Value.MovementStatus);
            expected[r.Key].TrackerExternalId.Should().Be(r.Value.TrackerExternalId);
            expected[r.Key].NumberOfSatellites.Should().Be(r.Value.NumberOfSatellites);

            if (expected[r.Key].LastMessageTime == null)
            {
                r.Value.LastMessageTime.Should().BeNull();
            }
            else
            {
                r.Value.LastMessageTime.Should().NotBeNull();
            }
        }
    }

    public static IEnumerable<object[]> LocationsData =>
        new List<object[]>
        {
            // пустой запрос
            new object[]
            {
                new int[] {},
                new Dictionary<int, (double Lat, double Long)>()
            },
            // одна машина
            new object[]
            {
                new [] {2552},
                new Dictionary<int, (double Lat, double Long)>
                {
                    { 2552, (42.432023, 54.556861) }
                }
            },
            // три машины
            new object[]
            {
                new [] { 2552, 1555, 128 },
                new Dictionary<int, (double Lat, double Long)>
                {
                    { 2552, (42.432023, 54.556861) },
                    { 1555, (36.4323, 28.55861) },
                    { 128, (39.4323, 12.55861) }
                }
            },
            // две машины и одна несуществующая
            new object[]
            {
                new [] { 2552, 1555, 100 },
                new Dictionary<int, (double Lat, double Long)>
                {
                    { 2552, (42.432023, 54.556861) },
                    { 1555, (36.4323, 28.55861) },
                }
            },
        };

    [Theory, MemberData(nameof(LocationsData))]
    public void GetLocations_ShouldReturnRightLocations(int[] externalIds,
        IDictionary<int, (double Lat, double Long)> expected)
    {
        _testOutputHelper.WriteLine("External IDs: " + string.Join(" ", externalIds));

        var results = TrackerMessageRepositoryMock.GetStub().GetLocations(externalIds);

        Assert.Equal(expected.Count, results.Count);

        results.Keys.Should().BeEquivalentTo(expected.Keys);

        foreach (var r in results)
        {
            expected[r.Key].Lat.Should().Be(r.Value.Lat);
            expected[r.Key].Long.Should().Be(r.Value.Long);
        }
    }

    public static IEnumerable<object[]> TracksData {
        get
        {
            var today = DateTime.Today;
            SystemTime.Set(today.AddHours(14));

            return new List<object[]>
            {
                // пустой запрос
                new object[]
                {
                    new int[] {},
                    new Dictionary<int, TrackPointInfo[]>()
                },
                // одна машина
                new object[]
                {
                    new [] {2552},
                    new Dictionary<int, TrackPointInfo[]>
                    {
                        { 2552, new TrackPointInfo[]
                            {
                                new TrackPointInfo{
                                    MessageId = 1,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
                                    Latitude = 49.432023,
                                    Longitude = 52.556861,
                                    NumberOfSatellites = 12,
                                    Speed = null
                                },
                                new TrackPointInfo{
                                    MessageId = 2,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
                                    Latitude = 42.432023,
                                    Longitude = 54.556861,
                                    NumberOfSatellites = 14,
                                    Speed = 12.1
                                },
                            }
                        }
                    }
                },
                // три машины
                new object[]
                {
                    new [] { 2552, 1555, 128 },
                    new Dictionary<int, TrackPointInfo[]>
                    {
                        { 2552, new TrackPointInfo[]
                            {
                                new TrackPointInfo{
                                    MessageId = 1,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
                                    Latitude = 49.432023,
                                    Longitude = 52.556861,
                                    NumberOfSatellites = 12,
                                    Speed = null
                                },
                                new TrackPointInfo{
                                    MessageId = 2,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
                                    Latitude = 42.432023,
                                    Longitude = 54.556861,
                                    NumberOfSatellites = 14,
                                    Speed = 12.1
                                },
                            }
                        },
                        { 1555, new TrackPointInfo[]
                            {
                                new TrackPointInfo{
                                    MessageId = 4,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
                                    Latitude = 36.4323,
                                    Longitude = 28.55861,
                                    NumberOfSatellites = 1,
                                    Speed = null
                                }
                            }
                        },
                        { 128, new TrackPointInfo[]
                            {
                                new TrackPointInfo{
                                    MessageId = 5,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(10),
                                    Latitude = 39.4323,
                                    Longitude = 12.55861,
                                    NumberOfSatellites = 19,
                                    Speed = 0
                                }
                            }
                        }
                    }
                },
                // две машины и одна несуществующая
                new object[]
                {
                    new [] { 2552, 1555, 100 },
                    new Dictionary<int, TrackPointInfo[]>
                    {
                        { 2552, new TrackPointInfo[]
                            {
                                new TrackPointInfo{
                                    MessageId = 1,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
                                    Latitude = 49.432023,
                                    Longitude = 52.556861,
                                    NumberOfSatellites = 12,
                                    Speed = null
                                },
                                new TrackPointInfo{
                                    MessageId = 2,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
                                    Latitude = 42.432023,
                                    Longitude = 54.556861,
                                    NumberOfSatellites = 14,
                                    Speed = 12.1
                                },
                            }
                        },
                        { 1555, new TrackPointInfo[]
                            {
                                new TrackPointInfo{
                                    MessageId = 4,
                                    Time = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
                                    Latitude = 36.4323,
                                    Longitude = 28.55861,
                                    NumberOfSatellites = 1,
                                    Speed = null
                                }
                            }
                        }
                    }
                }
            };
        }
    }

    [Theory, MemberData(nameof(TracksData))]
    public void GetTracks_ShouldReturnRightTracks(int[] externalIds,
        IDictionary<int, TrackPointInfo[]> expected)
    {
        _testOutputHelper.WriteLine("External IDs: " + string.Join(" ", externalIds));

        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));

        var results = TrackerMessageRepositoryMock.GetStub().GetTracks(today.ToUniversalTime(),
            DateTime.MaxValue, externalIds);

        Assert.Equal(expected.Count, results.Count);

        results.Keys.Should().BeEquivalentTo(expected.Keys);

        foreach (var r in results)
        {
            expected[r.Key].Should().BeEquivalentTo(r.Value);
        }
    }

    #endregion

}
