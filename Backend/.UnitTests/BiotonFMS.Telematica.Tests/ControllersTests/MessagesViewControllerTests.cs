using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos.MessagesView;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Mapping;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

/// <summary>
/// Тесты для <see cref="MessagesViewController"/>
/// </summary>
public class MessagesViewControllerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public MessagesViewControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public static IEnumerable<object[]> CriterionData =>
        new List<object[]>
        {
            // поиск по вхождению в начале названия машины, должна быть найдена одна машина
            new object[]
            {
                "красн",
                new[]
                {
                    new MessagesViewVehicleDto
                    {
                        Id = 1,
                        Name = "Красная машина",
                    }
                }
            },
            // поиск по вхождению в конец названия, должно быть найдено две машины
            new object[]
            {
                "ая",
                new[]
                {
                    new MessagesViewVehicleDto
                    {
                        Id = 1,
                        Name = "Красная машина"
                    },
                    new MessagesViewVehicleDto
                    {
                        Id = 3,
                        Name = "Желтая машина",
                    }
                }
            },
            new object[]
            {
                "64128256",
                new[]
                {
                    new MessagesViewVehicleDto
                    {
                        Id = 3,
                        Name = "Желтая машина",
                    }
                }
            },
            new object[]
            {
                "6412",
                Array.Empty<MessagesViewVehicleDto>()
            },
            new object[]
            {
                "15",
                new[]
                {
                    new MessagesViewVehicleDto
                    {
                        Id = 2,
                        Name = "Синяя машина",
                    }
                }
            },
            new object[]
            {
                "",
                new[]
                {
                    new MessagesViewVehicleDto
                    {
                        Id = 1,
                        Name = "Красная машина",
                    },
                    new MessagesViewVehicleDto
                    {
                        Id = 3,
                        Name = "Желтая машина",
                    },
                    new MessagesViewVehicleDto
                    {
                        Id = 2,
                        Name = "Синяя машина",
                    },
                    new MessagesViewVehicleDto
                    {
                        Id = 4,
                        Name = "Чёрный трактор",
                    }
                }
            },
            new object[]
            {
                "такой строки нет",
                Array.Empty<MessagesViewVehicleDto>()
            },
            new object[]
            {
                null!,
                new[]
                {
                    new MessagesViewVehicleDto { Id = 1, Name = "Красная машина" },
                    new MessagesViewVehicleDto { Id = 3, Name = "Желтая машина" },
                    new MessagesViewVehicleDto { Id = 2, Name = "Синяя машина" },
                    new MessagesViewVehicleDto { Id = 4, Name = "Чёрный трактор" }
                }
            }
        };

    [Theory, MemberData(nameof(CriterionData))]
    public void FindVehicles(string? criterion, MessagesViewVehicleDto[] expected)
    {
        _testOutputHelper.WriteLine("Criterion = \"" + criterion + "\"");
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));

        var result = GetController().FindVehicles(criterion);

        var actual = result.As<OkObjectResult>().Value.As<MessagesViewVehicleDto[]>();

        Assert.Equal(expected.Length, actual.Length);

        foreach (var a in actual)
        {
            var e = expected.FirstOrDefault(x => x.Id == a.Id);
            e.Should().NotBeNull();
            a.Id.Should().Be(e.Id);
            a.Name.Should().Be(e.Name);
        }
    }

    public static IEnumerable<object[]> StatisticsData =>
        new List<object[]>
        {
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 1,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = DateTime.UtcNow.AddHours(-100),
                    PeriodEnd = DateTime.UtcNow.AddHours(100)
                },
                new OkObjectResult(new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 2,
                    TotalTime = 10,
                    AverageSpeed = 12.1,
                    MaxSpeed = 12.1
                })
            },
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 1,
                    ViewMessageType = ViewMessageTypeEnum.CommandMessage,
                    PeriodStart = DateTime.UnixEpoch.AddHours(-100),
                    PeriodEnd = DateTime.UnixEpoch.AddHours(100)
                },
                new OkObjectResult(new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 3,
                    TotalTime = 20
                })
            },
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = -91001,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = DateTime.UtcNow,
                    PeriodEnd = DateTime.UtcNow
                },
                new NotFoundObjectResult("Трекер машины с таким id не существует")
            },
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 5,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = DateTime.UtcNow,
                    PeriodEnd = DateTime.UtcNow
                },
                new NotFoundObjectResult("Трекер машины с таким id не существует")
            },
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 2,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = DateTime.UtcNow,
                    PeriodEnd = DateTime.UtcNow
                },
                new OkObjectResult(new ViewMessageStatisticsDto())
            }
        };

    [Theory, MemberData(nameof(StatisticsData))]
    public void GetMessagesViewStatistics(MessagesViewStatisticsRequest request,
        ObjectResult expected)
    {
        _testOutputHelper.WriteLine("Request:\n" + JsonSerializer.Serialize(request, _jsonOptions));

        var result = GetController().GetMessagesViewStatistics(request);

        expected.StatusCode.Should().Be(result.As<ObjectResult>().StatusCode);

        expected.Value.Should().BeEquivalentTo(result.As<ObjectResult>().Value);
    }

    public static IEnumerable<object[]> TrackData =>
        new List<object[]>
        {
            new object[]
            {
                1, DateTime.UtcNow.AddHours(-100), DateTime.UtcNow.AddHours(100),
                new OkObjectResult(new MessagesViewTrackResponse
                {
                    ViewBounds = new ViewBounds
                    {
                        UpperLeftLatitude = 49.782023,
                        UpperLeftLongitude = 52.456860999999996,
                        BottomRightLatitude = 42.082023,
                        BottomRightLongitude = 54.656861
                    },
                    Track = new List<TrackPointInfo>
                    {
                        new()
                        {
                            MessageId = 1,
                            Latitude = 49.432023,
                            Longitude = 52.556861,
                            Speed = null,
                            NumberOfSatellites = 12
                        },
                        new()
                        {
                            MessageId = 2,
                            Latitude = 42.432023,
                            Longitude = 54.556861,
                            Speed = 12.1,
                            NumberOfSatellites = 14
                        }
                    }
                })
            },
            new object[]
            {
                -13432, DateTime.UtcNow.AddHours(-100), DateTime.UtcNow.AddHours(100),
                new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
            },
            new object[]
            {
                5, DateTime.UtcNow.AddHours(-100), DateTime.UtcNow.AddHours(100),
                new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
            },
            new object[]
            {
                2, DateTime.UtcNow.AddHours(-100), DateTime.UtcNow.AddHours(100),
                new OkObjectResult(new MessagesViewTrackResponse())
            }
        };

    [Theory, MemberData(nameof(TrackData))]
    public void GetMessagesViewTrack(int vehicleId,
        DateTime periodStart, DateTime periodEnd, ObjectResult expected)
    {
        _testOutputHelper.WriteLine($"id: {vehicleId}; dates: {periodStart} - {periodEnd}");

        var result = GetController().GetMessagesViewTrack(vehicleId, periodStart, periodEnd);
        
        expected.StatusCode.Should().Be(result.As<ObjectResult>().StatusCode);
        
        if (expected.StatusCode == 200)
        {
            var actual = result.As<OkObjectResult>().Value.As<MessagesViewTrackResponse>();
            
            expected.Value.As<MessagesViewTrackResponse>().Track
                .Should().Equal(actual.Track, (x, y) =>
                    x.Longitude.Equals(y.Longitude) &&
                    x.Latitude.Equals(y.Latitude) &&
                    x.MessageId.Equals(y.MessageId) &&
                    x.NumberOfSatellites.Equals(y.NumberOfSatellites) &&
                    x.Speed.Equals(y.Speed));
            
            expected.Value.As<MessagesViewTrackResponse>().ViewBounds
                .Should().BeEquivalentTo(actual.ViewBounds);
        }
        else
        {
            expected.Value.Should().BeEquivalentTo(result.As<ObjectResult>().Value);
        }
    }

    private static MessagesViewController GetController(
        ICollection<Vehicle>? vehicles = null,
        ICollection<TrackerMessage>? messages = null)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MessagesViewMappingProfile())));
        var logger = new Mock<ILogger<MessagesViewController>>().Object;
        var vehicleRepository = VehicleRepositoryMock.GetStub(vehicles);
        var commandRepository = TrackerCommandRepositoryMock.GetStub();
        var messageRepository = TrackerMessageRepositoryMock.GetStub(messages);

        return new MessagesViewController(logger, mapper, vehicleRepository, commandRepository, messageRepository);
    }
}