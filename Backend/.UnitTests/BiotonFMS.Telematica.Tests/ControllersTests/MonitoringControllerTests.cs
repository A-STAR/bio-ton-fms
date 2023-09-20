using AutoMapper;
using BioTonFMS.Common.Settings;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Mapping;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain.TrackerMessages;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

/// <summary>
/// Тесты для <see cref="MonitoringController"/>
/// </summary>
public class MonitoringControllerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MonitoringControllerTests(ITestOutputHelper testOutputHelper)
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
                new []
                {
                    new MonitoringVehicleDto
                    {
                        Id = 1,
                        Name = "Красная машина",
                        NumberOfSatellites = 14,
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 1,
                            Imei = "123",
                            ExternalId = 2552
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Moving,
                        LastMessageTime = DateTime.UtcNow
                    }
                }
            },
            // поиск по вхождению в конец названия, должно быть найдено две машины
            new object[]
            {
                "ая",
                new []
                {
                    new MonitoringVehicleDto
                    {
                        Id = 1,
                        Name = "Красная машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 1,
                            Imei = "123",
                            ExternalId = 2552
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Moving,
                        LastMessageTime = SystemTime.UtcNow,
                        NumberOfSatellites = 14
                    },
                    new MonitoringVehicleDto
                    {
                        Id = 3,
                        Name = "Желтая машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 3,
                            Imei = "64128256",
                            ExternalId = 128
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Stopped,
                        NumberOfSatellites = 19
                    }
                }
            },
            new object[]
            {
                "64128256",
                new []
                {
                    new MonitoringVehicleDto
                    {
                        Id = 3,
                        Name = "Желтая машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 3,
                            Imei = "64128256",
                            ExternalId = 128
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Stopped,
                        NumberOfSatellites = 19
                    }
                }
            },
            new object[]
            {
                "6412",
                Array.Empty<MonitoringVehicleDto>()
            },
            new object[]
            {
                "15",
                new []
                {
                    new MonitoringVehicleDto
                    {
                        Id = 2,
                        Name = "Синяя машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 2,
                            Imei = "128128",
                            ExternalId = 15
                        },
                        ConnectionStatus = ConnectionStatusEnum.NotConnected,
                        MovementStatus = MovementStatusEnum.NoData
                    }
                }
            },
            new object[]
            {
                "",
                new []
                {
                    new MonitoringVehicleDto
                    {
                        Id = 1,
                        Name = "Красная машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 1,
                            Imei = "123",
                            ExternalId = 2552
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Moving,
                        LastMessageTime = SystemTime.UtcNow,
                        NumberOfSatellites = 14
                    },
                    new MonitoringVehicleDto
                    {
                        Id = 3,
                        Name = "Желтая машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 3,
                            Imei = "64128256",
                            ExternalId = 128
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Stopped,
                        NumberOfSatellites = 19
                    },
                    new MonitoringVehicleDto
                    {
                        Id = 2,
                        Name = "Синяя машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 2,
                            Imei = "128128",
                            ExternalId = 15
                        },
                        ConnectionStatus = ConnectionStatusEnum.NotConnected,
                        MovementStatus = MovementStatusEnum.NoData
                    },
                    new MonitoringVehicleDto
                    {
                        Id = 4,
                        Name = "Чёрный трактор",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 4,
                            Imei = "6412825699",
                            ExternalId = 1555
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.NoData,
                        NumberOfSatellites = 1,
                        LastMessageTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20)
                    }
                }
            },
            new object[]
            {
                "такой строки нет",
                Array.Empty<MonitoringVehicleDto>()
            },
            new object[]
            {
                null!,
                new []
                {
                    new MonitoringVehicleDto
                    {
                        Id = 1,
                        Name = "Красная машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 1,
                            Imei = "123",
                            ExternalId = 2552
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Moving,
                        LastMessageTime = SystemTime.UtcNow,
                        NumberOfSatellites = 14
                    },
                    new MonitoringVehicleDto
                    {
                        Id = 3,
                        Name = "Желтая машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 3,
                            Imei = "64128256",
                            ExternalId = 128
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.Stopped,
                        NumberOfSatellites = 19
                    },
                    new MonitoringVehicleDto
                    {
                        Id = 2,
                        Name = "Синяя машина",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 2,
                            Imei = "128128",
                            ExternalId = 15
                        },
                        ConnectionStatus = ConnectionStatusEnum.NotConnected,
                        MovementStatus = MovementStatusEnum.NoData
                    },
                    new MonitoringVehicleDto
                    {
                        Id = 4,
                        Name = "Чёрный трактор",
                        Tracker = new MonitoringTrackerDto
                        {
                            Id = 4,
                            Imei = "6412825699",
                            ExternalId = 1555
                        },
                        ConnectionStatus = ConnectionStatusEnum.Connected,
                        MovementStatus = MovementStatusEnum.NoData,
                        NumberOfSatellites = 1,
                        LastMessageTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20)
                    }
                }
            }
        };
    
    [Theory, MemberData(nameof(CriterionData))]
    public void FindVehicles(string? criterion, MonitoringVehicleDto[] expected)
    {
        _testOutputHelper.WriteLine("Criterion = \"" + criterion + "\"");
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));

        var result = GetController().FindVehicles(criterion);

        var actual = result.As<OkObjectResult>().Value.As<MonitoringVehicleDto[]>();
        
        Assert.Equal(expected.Length, actual.Length);

        foreach (var a in actual)
        {
            var e = expected.FirstOrDefault(x => x.Id == a.Id);
            e.Should().NotBeNull();
            
            a.ConnectionStatus.Should().Be(e!.ConnectionStatus);
            a.MovementStatus.Should().Be(e.MovementStatus);
            a.NumberOfSatellites.Should().Be(e.NumberOfSatellites);

            if (e.LastMessageTime == null)
            {
                a.LastMessageTime.Should().BeNull();
            }
            else
            {
                a.LastMessageTime.Should().NotBeNull();
            }
            
            a.Tracker.Should().BeEquivalentTo(e.Tracker);
            a.Id.Should().Be(e.Id);
            a.Name.Should().Be(e.Name);
        }
    }

    [Fact]
    public void LocationsAndTracks_ShoulReturnLocationFromLastMessage_ForOneVehicle()
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));
        var vehicle = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 1).Single();

        var request = new LocationAndTrackRequest
        {
            VehicleId = 1,
            NeedReturnTrack = false
        };
        var messages = TrackerMessageRepositoryMock.Messages;
        var lastMessage = messages.Where(m => m.ExternalTrackerId == vehicle!.Tracker!.ExternalId ).OrderBy(m => m.ServerDateTime).Last();

        var actionResult = GetController(messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request } );
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(1);
        response.Tracks.First().VehicleId.Should().Be(vehicle.Id);
        response.Tracks.First().VehicleName.Should().Be(vehicle.Name);
        response.Tracks.First().Track.Length.Should().Be(0);
        response.Tracks.First().Latitude.Should().Be(lastMessage.Latitude);
        response.Tracks.First().Longitude.Should().Be(lastMessage.Longitude);
    }

    [Fact]
    public void LocationsAndTracks_ShoulReturnCorrectViewBounds_ForOneVehicle()
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));
        var vehicle = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 1).Single();

        var request = new LocationAndTrackRequest
        {
            VehicleId = 1,
            NeedReturnTrack = false
        };
        var messages = TrackerMessageRepositoryMock.Messages;
        var lastMessage = messages.Where(m => m.ExternalTrackerId == vehicle!.Tracker!.ExternalId).OrderBy(m => m.ServerDateTime).Last();

        var actionResult = GetController(messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request });
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(1);
        response.ViewBounds.Should().NotBeNull();
        response.ViewBounds!.UpperLeftLatitude.Should().Be(lastMessage.Latitude + MonitoringController.DefaultDifLat);
        response.ViewBounds!.UpperLeftLongitude.Should().Be(lastMessage.Longitude - MonitoringController.DefaultDifLon);
        response.ViewBounds!.BottomRightLatitude.Should().Be(lastMessage.Latitude - MonitoringController.DefaultDifLat);
        response.ViewBounds!.BottomRightLongitude.Should().Be(lastMessage.Longitude + MonitoringController.DefaultDifLon);
    }

    [Fact]
    public void LocationsAndTracks_ShoulReturnCorrectViewBounds_For3Vehicles()
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));
        var vehicle1 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 1).Single();
        var vehicle2 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 4).Single();
        var vehicle3 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 3).Single();

        var request1 = new LocationAndTrackRequest
        {
            VehicleId = 1,
            NeedReturnTrack = false
        };
        var request2 = new LocationAndTrackRequest
        {
            VehicleId = 4,
            NeedReturnTrack = false
        };
        var request3 = new LocationAndTrackRequest
        {
            VehicleId = 3,
            NeedReturnTrack = false
        };

        var messages = TrackerMessageRepositoryMock.Messages;
        var lastMessage1 = messages.Where(m => m.ExternalTrackerId == vehicle1!.Tracker!.ExternalId).OrderBy(m => m.ServerDateTime).Last();
        var lastMessage2 = messages.Where(m => m.ExternalTrackerId == vehicle2!.Tracker!.ExternalId).OrderBy(m => m.ServerDateTime).Last();
        var lastMessage3 = messages.Where(m => m.ExternalTrackerId == vehicle3!.Tracker!.ExternalId).OrderBy(m => m.ServerDateTime).Last();

        List<double> lats = new()
        {
            lastMessage1.Latitude!.Value,
            lastMessage2.Latitude!.Value,
            lastMessage3.Latitude!.Value
        };

        List<double> lons = new()
        {
            lastMessage1.Longitude!.Value,
            lastMessage2.Longitude!.Value,
            lastMessage3.Longitude!.Value
        };
        var difLat = (lats.Max() - lats.Min()) / 20;
        var difLon = (lons.Max() - lons.Min()) / 20;
        if (difLat < MonitoringController.DefaultDifLat)
        {
            difLat = MonitoringController.DefaultDifLat;
        }
        if (difLon < MonitoringController.DefaultDifLon)
        {
            difLon = MonitoringController.DefaultDifLon;
        }
        var expectedViewBounds = new ViewBounds
        {
            UpperLeftLatitude = lats.Max() + difLat,
            UpperLeftLongitude = lons.Min() - difLon,
            BottomRightLatitude = lats.Min() - difLat,
            BottomRightLongitude = lons.Max() + difLon
        };

        var actionResult = GetController(messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request1,  request2, request3 });
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(3);
        response.ViewBounds.Should().NotBeNull();
        response.ViewBounds.Should().BeEquivalentTo(expectedViewBounds);
    }

    [Fact]
    public void LocationsAndTracks_ShoulReturnCorrectTrack_ForVehicleRequiresTrack()
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));
        var vehicle1 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 1).Single();
        var vehicle2 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 4).Single();
        var vehicle3 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 3).Single();

        var request1 = new LocationAndTrackRequest
        {
            VehicleId = 1,
            NeedReturnTrack = true
        };
        var request2 = new LocationAndTrackRequest
        {
            VehicleId = 4,
            NeedReturnTrack = false
        };
        var request3 = new LocationAndTrackRequest
        {
            VehicleId = 3,
            NeedReturnTrack = false
        };

        var messages = TrackerMessageRepositoryMock.Messages;
        var expectedTrack = new TrackPointInfo[]
        {
            new TrackPointInfo
            {
                MessageId = 1,
                Time = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
                Latitude = 49.432023,
                Longitude = 52.556861,
                Speed = null,
                NumberOfSatellites = 12
            },
            new TrackPointInfo
            {
                MessageId = 2,
                Time = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
                Latitude = 49.432023,
                Longitude = 52.556861,
                Speed = 12.1,
                NumberOfSatellites = 14
            }
        };

        var actionResult = GetController(messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request1, request2, request3 });
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(3);
        response.ViewBounds.Should().NotBeNull();
        var responseWithTrack = response.Tracks.Where(t => t.VehicleId == 1).Single();
        responseWithTrack.Track.Length.Should().Be(2);
    }

    [Fact]
    public void LocationsAndTracks_ShoulReturnCorrectViewBounds_For3VehiclesWithTrack()
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));
        var vehicle1 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 1).Single();
        var vehicle2 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 4).Single();
        var vehicle3 = VehicleRepositoryMock.SampleVehicles.Where(v => v.Id == 3).Single();

        var request1 = new LocationAndTrackRequest
        {
            VehicleId = 1,
            NeedReturnTrack = true
        };
        var request2 = new LocationAndTrackRequest
        {
            VehicleId = 4,
            NeedReturnTrack = false
        };
        var request3 = new LocationAndTrackRequest
        {
            VehicleId = 3,
            NeedReturnTrack = false
        };

        var messages = TrackerMessageRepositoryMock.Messages;
        var lastMessage1 = messages.Where(m => m.ExternalTrackerId == vehicle1!.Tracker!.ExternalId).OrderBy(m => m.ServerDateTime).Last();
        var lastMessage2 = messages.Where(m => m.ExternalTrackerId == vehicle2!.Tracker!.ExternalId).OrderBy(m => m.ServerDateTime).Last();
        var lastMessage3 = messages.Where(m => m.ExternalTrackerId == vehicle3!.Tracker!.ExternalId).OrderBy(m => m.ServerDateTime).Last();

        var trackMessages = messages.Where(m => m.ExternalTrackerId == vehicle1!.Tracker!.ExternalId && m.ServerDateTime > today.ToUniversalTime()).ToArray();

        List<double> lats = new()
        {
            lastMessage1.Latitude!.Value,
            lastMessage2.Latitude!.Value,
            lastMessage3.Latitude!.Value
        };
        lats.AddRange(trackMessages.Select(m => m.Latitude!.Value));

        List<double> lons = new()
        {
            lastMessage1.Longitude!.Value,
            lastMessage2.Longitude!.Value,
            lastMessage3.Longitude!.Value
        };
        lons.AddRange(trackMessages.Select(m => m.Longitude!.Value));

        var difLat = (lats.Max() - lats.Min()) / 20;
        var difLon = (lons.Max() - lons.Min()) / 20;
        if (difLat < MonitoringController.DefaultDifLat)
        {
            difLat = MonitoringController.DefaultDifLat;
        }
        if (difLon < MonitoringController.DefaultDifLon)
        {
            difLon = MonitoringController.DefaultDifLon;
        }
        var expectedViewBounds = new ViewBounds
        {
            UpperLeftLatitude = lats.Max() + difLat,
            UpperLeftLongitude = lons.Min() - difLon,
            BottomRightLatitude = lats.Min() - difLat,
            BottomRightLongitude = lons.Max() + difLon
        };

        var actionResult = GetController(messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request1, request2, request3 });
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(3);
        response.ViewBounds.Should().NotBeNull();
        response.ViewBounds.Should().BeEquivalentTo(expectedViewBounds);
    }

    private static MonitoringController GetController(ICollection<TrackerMessage>? messages = null)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MonitoringMappingProfile())));
        var logger = new Mock<ILogger<MonitoringController>>().Object;
        var vehicleRepository = VehicleRepositoryMock.GetStub();
        var trackerMessageRepository = TrackerMessageRepositoryMock.GetStub(messages);
        var options = Options.Create(new TrackerOptions { TrackerAddressValidMinutes = 60 });
        var tagsRepository = TrackerTagRepositoryMock.GetStub();
        var trackerRepository = TrackerRepositoryMock.GetStub();

        return new MonitoringController(mapper, logger, vehicleRepository,
            trackerMessageRepository, options, tagsRepository, trackerRepository);
    }
}