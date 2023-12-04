using AutoMapper;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using BioTonFMS.Common.Settings;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Mapping;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections;
using BioTonFMS.Domain.MessagesView;
using Xunit.Abstractions;

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

        var result = GetController(MonitoringVehicles).FindVehicles(criterion);

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
    public void LocationsAndTracks_ShouldReturnLocationFromLastMessage_ForOneVehicle()
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

        var actionResult = GetController(null, messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request });
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
    public void LocationsAndTracks_ShouldReturnCorrectViewBounds_ForOneVehicle()
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

        var actionResult = GetController(null, messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request });
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(1);
        response.ViewBounds.Should().NotBeNull();
        response.ViewBounds!.UpperLeftLatitude.Should().Be(lastMessage.Latitude + TelematicaHelpers.DefaultDifLat);
        response.ViewBounds!.UpperLeftLongitude.Should().Be(lastMessage.Longitude - TelematicaHelpers.DefaultDifLon);
        response.ViewBounds!.BottomRightLatitude.Should().Be(lastMessage.Latitude - TelematicaHelpers.DefaultDifLat);
        response.ViewBounds!.BottomRightLongitude.Should().Be(lastMessage.Longitude + TelematicaHelpers.DefaultDifLon);
    }

    [Fact]
    public void LocationsAndTracks_ShouldReturnCorrectViewBounds_For3Vehicles()
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
        if (difLat < TelematicaHelpers.DefaultDifLat)
        {
            difLat = TelematicaHelpers.DefaultDifLat;
        }
        if (difLon < TelematicaHelpers.DefaultDifLon)
        {
            difLon = TelematicaHelpers.DefaultDifLon;
        }
        var expectedViewBounds = new ViewBounds
        {
            UpperLeftLatitude = lats.Max() + difLat,
            UpperLeftLongitude = lons.Min() - difLon,
            BottomRightLatitude = lats.Min() - difLat,
            BottomRightLongitude = lons.Max() + difLon
        };

        var actionResult = GetController(null, messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request1, request2, request3 });
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(3);
        response.ViewBounds.Should().NotBeNull();
        response.ViewBounds.Should().BeEquivalentTo(expectedViewBounds);
    }

    [Fact]
    public void LocationsAndTracks_ShouldReturnCorrectTrack_ForVehicleRequiresTrack()
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

        var actionResult = GetController(null, messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request1, request2, request3 });
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(3);
        response.ViewBounds.Should().NotBeNull();
        var responseWithTrack = response.Tracks.Where(t => t.VehicleId == 1).Single();
        responseWithTrack.Track.Length.Should().Be(2);
    }

    [Fact]
    public void LocationsAndTracks_ShouldReturnCorrectViewBounds_For3VehiclesWithTrack()
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
        if (difLat < TelematicaHelpers.DefaultDifLat)
        {
            difLat = TelematicaHelpers.DefaultDifLat;
        }
        if (difLon < TelematicaHelpers.DefaultDifLon)
        {
            difLon = TelematicaHelpers.DefaultDifLon;
        }
        var expectedViewBounds = new ViewBounds
        {
            UpperLeftLatitude = lats.Max() + difLat,
            UpperLeftLongitude = lons.Min() - difLon,
            BottomRightLatitude = lats.Min() - difLat,
            BottomRightLongitude = lons.Max() + difLon
        };

        var actionResult = GetController(null, messages).LocationsAndTracks(today.ToUniversalTime(), new LocationAndTrackRequest[] { request1, request2, request3 });
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as LocationsAndTracksResponse;

        response.Should().NotBeNull();
        response!.Tracks.Should().HaveCount(3);
        response.ViewBounds.Should().NotBeNull();
        response.ViewBounds.Should().BeEquivalentTo(expectedViewBounds);
    }

    [Fact]
    public void GetVehicleInformation_ShouldReturnNotFound_ForVehicleWithoutTracker()
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));

        var vehicleWithoutTracker = MonitoringVehicles.Where(v => v.Id == 5).Single();

        var actionResult = GetController(MonitoringVehicles, MonitoringMessages).GetVehicleInformation(vehicleWithoutTracker.Id);
        var notFoundResult = actionResult as NotFoundResult;

        notFoundResult.Should().NotBeNull();
    }

    public static IEnumerable<object[]> VehicleInfoData
    {
        get
        {
            var today = DateTime.Today;
            SystemTime.Set(today.AddHours(14));

            return new List<object[]>
            {
                // Машина с сообщениями и без настроенных датчиков
                new object[]
                {
                    1,
                    new MonitoringVehicleInfoDto
                    {
                        GeneralInfo = new MonitoringGeneralInfoDto
                        {
                            LastMessageTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
                            Speed = 12.1,
                            Mileage = null,
                            EngineHours = null,
                            SatellitesNumber = 14,
                            Latitude = 42.432023,
                            Longitude = 54.556861
                        },
                        TrackerInfo = new MonitoringTrackerInfoDto
                        {
                            ExternalId = 2552,
                            Imei = "123",
                            SimNumber = "",
                            Parameters = new List<TrackerParameter>
                            {
                                new TrackerParameter {
                                    LastValueDecimal = 12345.0,
                                    ParamName = "rec_sn"
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 6.0,
                                    ParamName = "hdop"
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 2134.0,
                                    ParamName = "rs485_1"
                                }
                            }
                        }
                    }
                },
                // Машина без сообщений и без настроенных датчиков
                new object[]
                {
                    2,
                    new MonitoringVehicleInfoDto
                    {
                        GeneralInfo = new MonitoringGeneralInfoDto(),
                        TrackerInfo = new MonitoringTrackerInfoDto
                        {
                            ExternalId = 15,
                            Imei = "128128",
                            SimNumber = "",
                            Parameters = new List<TrackerParameter>()
                        }
                    }
                },
                // Машина без сообщений и с настроенными датчиками
                new object[]
                {
                    6,
                    new MonitoringVehicleInfoDto
                    {
                        GeneralInfo = new MonitoringGeneralInfoDto(),
                        TrackerInfo = new MonitoringTrackerInfoDto
                        {
                            ExternalId = 1444,
                            Imei = "6412825699",
                            SimNumber = "",
                            Parameters = new List<TrackerParameter>(),
                            Sensors = new List<TrackerSensorDto>
                            {
                                new TrackerSensorDto{
                                    Name = "Датчик 1",
                                    Unit = "Килограмм"
                                },
                                new TrackerSensorDto
                                {
                                    Name = "Датчик 2",
                                    Unit = "Метр",
                                }
                            }
                        }
                    }
                },
                // Машина с сообщениями и с настроенными датчиками
                new object[]
                {
                    3,
                    new MonitoringVehicleInfoDto
                    {
                        GeneralInfo = new MonitoringGeneralInfoDto
                        {
                            LastMessageTime = SystemTime.UtcNow - TimeSpan.FromSeconds(10),
                            Speed = 0,
                            Mileage = null,
                            EngineHours = null,
                            SatellitesNumber = 19,
                            Latitude = 39.4323,
                            Longitude = 12.55861
                        },
                        TrackerInfo = new MonitoringTrackerInfoDto
                        {
                            ExternalId = 128,
                            Imei = "64128256",
                            SimNumber = "",
                                                        Parameters = new List<TrackerParameter>
                            {
                                new TrackerParameter {
                                    LastValueDecimal = 12345.0,
                                    ParamName = "rec_sn"
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 6.0,
                                    ParamName = "hdop"
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 2134.0,
                                    ParamName = "rs485_1"
                                },
                                // следующие два параметра (датчики) появляются здесь из-за несовершенства тестовой модели репозитория (не реализована фильтрация для Fetch)
                                new TrackerParameter
                                {
                                    LastValueDecimal = 25,
                                    ParamName = ""
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 11,
                                    ParamName = ""
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 11,
                                    ParamName = ""
                                }
                            },
                            Sensors = new List<TrackerSensorDto>
                            {
                                new TrackerSensorDto{
                                    Name = "Датчик 1",
                                    Unit = "Килограмм",
                                    Value = "25"
                                },
                                new TrackerSensorDto
                                {
                                    Name = "Датчик 2",
                                    Unit = "Метр",
                                    Value = "11"
                                }
                            }
                        }
                    }
                }
            };
        }
    }

    [Theory, MemberData(nameof(VehicleInfoData))]
    public void GetVehicleInformation_ShouldReturnVehicleInformation_ForVehicleWithTrackerAndMessages(int vehicleId, MonitoringVehicleInfoDto expected )
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));

        var testVehicle = MonitoringGetInfoVehicles.Where(v => v.Id == vehicleId).Single();

        var actionResult = GetController(MonitoringGetInfoVehicles, MonitoringMessages).GetVehicleInformation(testVehicle.Id);
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as MonitoringVehicleInfoDto;

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetTrackPointInformation_ShouldReturnNotFound_IfMessageNotExists()
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));

        var actionResult = GetController(MonitoringVehicles, MonitoringMessages).GetTrackPointInformation(1000);
        var notFoundResult = actionResult as NotFoundResult;

        notFoundResult.Should().NotBeNull();
    }

    public static IEnumerable<object[]> TrackPointInfoData
    {
        get
        {
            var today = DateTime.Today;
            SystemTime.Set(today.AddHours(14));

            return new List<object[]>
            {
                // Сообщение без датчиков
                new object[]
                {
                    1,
                    new MonitoringTrackPointInfoDto
                    {
                        GeneralInfo = new TrackPointGeneralInfoDto
                        {
                            MessageTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
                            Speed = null,
                            NumberOfSatellites = 12,
                            Latitude = 49.432023,
                            Longitude = 52.556861
                        },
                        TrackerInfo = new TrackPointTrackerInfoDto
                        {
                            Parameters = new List<TrackerParameter>
                            {
                                new TrackerParameter {
                                    LastValueDecimal = 1234,
                                    ParamName = "rec_sn"
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 6.0,
                                    ParamName = "hdop"
                                },
                                new TrackerParameter
                                {
                                    LastValueString = "11101011",
                                    ParamName = "out"
                                }
                            }
                        }
                    }
                },
                // Сообщение c датчиками
                new object[]
                {
                    5,
                    new MonitoringTrackPointInfoDto
                    {
                        GeneralInfo = new TrackPointGeneralInfoDto
                        {
                            MessageTime = SystemTime.UtcNow - TimeSpan.FromSeconds(10),
                            Speed = 0,
                            NumberOfSatellites = 19,
                            Latitude = 39.4323,
                            Longitude = 12.55861
                        },
                        TrackerInfo = new TrackPointTrackerInfoDto
                        {
                            Parameters = new List<TrackerParameter>
                            {
                                new TrackerParameter {
                                    LastValueDecimal = 12345,
                                    ParamName = "rec_sn"
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 6.0,
                                    ParamName = "hdop"
                                },
                                new TrackerParameter
                                {
                                    LastValueDecimal = 2134,
                                    ParamName = "rs485_1"
                                },
                            },
                            Sensors = new List<TrackerSensorDto> {
                                new TrackerSensorDto
                                {
                                    Name = "Датчик 1",
                                    Unit = "Килограмм",
                                    Value = "25"
                                },
                                new TrackerSensorDto
                                {
                                    Name = "Датчик 2",
                                    Unit = "Метр",
                                    Value = "11"
                                }
                            }
                        }
                    }
                },
            };
        }
    }

    [Theory, MemberData(nameof(TrackPointInfoData))]
    public void GetTrackPointInformation_ShouldReturnTrackPointInformation_ForMessages(int messageId, MonitoringTrackPointInfoDto expected)
    {
        var today = DateTime.Today;
        SystemTime.Set(today.AddHours(14));

        var actionResult = GetController(MonitoringGetInfoVehicles, MonitoringMessages, GetTrackers()).GetTrackPointInformation(messageId);
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as MonitoringTrackPointInfoDto;

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expected);
    }

    private static MonitoringController GetController(
        ICollection<Vehicle>? vehicles = null,
        ICollection<TrackerMessage>? messages = null,
        ICollection<Tracker>? trackers = null)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MonitoringMappingProfile())));
        var logger = new Mock<ILogger<MonitoringController>>().Object;
        var vehicleRepository = VehicleRepositoryMock.GetStub(vehicles);
        var trackerMessageRepository = TrackerMessageRepositoryMock.GetStub(messages);
        var options = Options.Create(new TrackerOptions { TrackerAddressValidMinutes = 60 });
        var tagsRepository = TrackerTagRepositoryMock.GetStub();
        var trackerRepository = TrackerRepositoryMock.GetStub(trackers);

        return new MonitoringController(mapper, logger, vehicleRepository,
            trackerMessageRepository, options, tagsRepository, trackerRepository);
    }

    private static IList<Vehicle> MonitoringVehicles => new List<Vehicle>
    {
        new()
        {
            Id = 1,
            Name = "Красная машина",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            Description = "Описание 1",
            Make = "Ford",
            Model = "Focus",
            ManufacturingYear = 2020,
            RegistrationNumber = "В167АР 199",
            InventoryNumber = "1234",
            TrackerId = 1,
            Tracker = new Tracker
            {
                Id = 1,
                Imei = "123",
                ExternalId = 2552
            }
        },
        new()
        {
            Id = 2,
            Name = "Синяя машина",
            Type = VehicleTypeEnum.Agro,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            VehicleGroup = new VehicleGroup { Id = 1, Name = "Группа 1" },
            VehicleGroupId = 1,
            Description = "Описание 2",
            Make = "Ford",
            Model = "Focus",
            ManufacturingYear = 2015,
            RegistrationNumber = "В167АР 172",
            InventoryNumber = "1235",
            TrackerId = 2,
            Tracker = new Tracker
            {
                Id = 2,
                Imei = "128128",
                ExternalId = 15
            }
        },
        new()
        {
            Id = 3,
            Name = "Желтая машина",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Sprayer,
            FuelType = new FuelType { Id = 2, Name = "Дизель" },
            FuelTypeId = 2,
            VehicleGroup = new VehicleGroup { Id = 2, Name = "Группа 2" },
            VehicleGroupId = 2,
            Description = "Описание 3",
            Make = "Mazda",
            Model = "CX5",
            ManufacturingYear = 2010,
            RegistrationNumber = "В167АР 174",
            InventoryNumber = "1236",
            TrackerId = 3,
            Tracker = new Tracker
            {
                Id = 3,
                Imei = "64128256",
                ExternalId = 128
            }
        },
        new()
        {
            Id = 4,
            Name = "Чёрный трактор",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Sprayer,
            FuelType = new FuelType { Id = 2, Name = "Дизель" },
            FuelTypeId = 2,
            VehicleGroup = new VehicleGroup { Id = 2, Name = "Группа 2" },
            VehicleGroupId = 2,
            Description = "Описание 4",
            Make = "Mazda",
            Model = "CX6",
            ManufacturingYear = 2012,
            RegistrationNumber = "В187АР 163",
            InventoryNumber = "12367",
            TrackerId = 4,
            Tracker = new Tracker
            {
                Id = 4,
                Imei = "6412825699",
                ExternalId = 1555
            }
        },
        new()
        {
            Id = 5,
            Name = "Красная машина без трекера",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            Description = "Описание 1",
            Make = "Ford",
            Model = "Focus",
            ManufacturingYear = 2020,
            RegistrationNumber = "В167АР 189",
            InventoryNumber = "1234"
        },
    };

    private static IList<Vehicle> MonitoringGetInfoVehicles => new List<Vehicle>
    {
        new()
        {
            Id = 1,
            Name = "Красная машина",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            Description = "Описание 1",
            Make = "Ford",
            Model = "Focus",
            ManufacturingYear = 2020,
            RegistrationNumber = "В167АР 199",
            InventoryNumber = "1234",
            TrackerId = 1,
            Tracker = new Tracker
            {
                Id = 1,
                Imei = "123",
                ExternalId = 2552
            }
        },
        new()
        {
            Id = 2,
            Name = "Синяя машина",
            Type = VehicleTypeEnum.Agro,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            VehicleGroup = new VehicleGroup { Id = 1, Name = "Группа 1" },
            VehicleGroupId = 1,
            Description = "Описание 2",
            Make = "Ford",
            Model = "Focus",
            ManufacturingYear = 2015,
            RegistrationNumber = "В167АР 172",
            InventoryNumber = "1235",
            TrackerId = 2,
            Tracker = new Tracker
            {
                Id = 2,
                Imei = "128128",
                ExternalId = 15
            }
        },
        new()
        {
            Id = 3,
            Name = "Желтая машина",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Sprayer,
            FuelType = new FuelType { Id = 2, Name = "Дизель" },
            FuelTypeId = 2,
            VehicleGroup = new VehicleGroup { Id = 2, Name = "Группа 2" },
            VehicleGroupId = 2,
            Description = "Описание 3",
            Make = "Mazda",
            Model = "CX5",
            ManufacturingYear = 2010,
            RegistrationNumber = "В167АР 174",
            InventoryNumber = "1236",
            TrackerId = 3,
            Tracker = new Tracker
            {
                Id = 3,
                Imei = "64128256",
                ExternalId = 128,
                Sensors = new List<Sensor>
                {
                    new Sensor
                    {
                        Id = 3,
                        Name = "Датчик 1",
                        IsVisible = true,
                        UnitId = 1,
                        Unit = new Unit(1, "Килограмм", "кг")
                    },
                    new Sensor
                    {
                        Id = 4,
                        Name = "Датчик 2",
                        IsVisible = true,
                        UnitId = 2,
                        Unit = new Unit(2, "Метр", "м")
                    }
                }
            }
        },
        new()
        {
            Id = 4,
            Name = "Чёрный трактор",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Sprayer,
            FuelType = new FuelType { Id = 2, Name = "Дизель" },
            FuelTypeId = 2,
            VehicleGroup = new VehicleGroup { Id = 2, Name = "Группа 2" },
            VehicleGroupId = 2,
            Description = "Описание 4",
            Make = "Mazda",
            Model = "CX6",
            ManufacturingYear = 2012,
            RegistrationNumber = "В187АР 163",
            InventoryNumber = "12367",
            TrackerId = 4,
            Tracker = new Tracker
            {
                Id = 4,
                Imei = "6412825699",
                ExternalId = 1555
            }
        },
        new()
        {
            Id = 5,
            Name = "Красная машина без трекера",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Car,
            FuelType = new FuelType { Id = 1, Name = "Бензин" },
            FuelTypeId = 1,
            Description = "Описание 1",
            Make = "Ford",
            Model = "Focus",
            ManufacturingYear = 2020,
            RegistrationNumber = "В167АР 189",
            InventoryNumber = "1234"
        },
        new()
        {
            Id = 6,
            Name = "Чёрный трактор 6",
            Type = VehicleTypeEnum.Transport,
            VehicleSubType = VehicleSubTypeEnum.Sprayer,
            FuelType = new FuelType { Id = 2, Name = "Дизель" },
            FuelTypeId = 2,
            VehicleGroup = new VehicleGroup { Id = 2, Name = "Группа 2" },
            VehicleGroupId = 2,
            Description = "Описание 4",
            Make = "Mazda",
            Model = "CX6",
            ManufacturingYear = 2012,
            RegistrationNumber = "В187АР 163",
            InventoryNumber = "12367",
            TrackerId = 6,
            Tracker = new Tracker
            {
                Id = 6,
                Imei = "6412825699",
                ExternalId = 1444,
                Sensors = new List<Sensor>
                {
                    new Sensor
                    {
                        Id = 1,
                        Name = "Датчик 1",
                        IsVisible = true,
                        UnitId = 1,
                        Unit = new Unit(1, "Килограмм", "кг")
                    },
                    new Sensor
                    {
                        Id = 2,
                        Name = "Датчик 2",
                        IsVisible = true,
                        UnitId = 2,
                        Unit = new Unit(2, "Метр", "м")
                    }
                }
            }
        },
    };

    public static TrackerMessage[] MonitoringMessages => new TrackerMessage[]
    {
        new()
        {
            Id = 1,
            ExternalTrackerId = 2552,
            Imei = "123",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
            Latitude = 49.432023,
            Longitude = 52.556861,
            SatNumber = 12,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
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
            Id = 2,
            ExternalTrackerId = 2552,
            Imei = "123",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            Latitude = 42.432023,
            Longitude = 54.556861,
            SatNumber = 14,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
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
        },
        new()
        {
            Id = 3,
            ExternalTrackerId = 1024,
            Imei = "512128256",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
            Latitude = 39.4323,
            Longitude = 12.55861,
            SatNumber = 1,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 92.0,
            Speed = null,
            Direction = 2.1,
            FuelLevel = 90,
            CoolantTemperature = 40,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("719C3996-DB32-4777-A4F5-BC0D8A9E3B96"),
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
        },
        new()
        {
            Id = 4,
            ExternalTrackerId = 1555,
            Imei = "6412825699",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
            Latitude = 36.4323,
            Longitude = 28.55861,
            SatNumber = 1,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 92.0,
            Speed = null,
            Direction = 2.1,
            FuelLevel = 90,
            CoolantTemperature = 40,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("719C3996-DB32-4777-A4F5-BC0D8A9E3B96"),
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
        },
        new()
        {
            Id = 5,
            ExternalTrackerId = 128,
            Imei = "64128256",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(10),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(10),
            Latitude = 39.4323,
            Longitude = 12.55861,
            SatNumber = 19,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGsm,
            Altitude = 92.0,
            Speed = 0,
            Direction = 2.1,
            FuelLevel = 90,
            CoolantTemperature = 40,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("719C3996-DB32-4777-A4F5-BC0D8A9E3B96"),
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
                },
                new MessageTagInteger
                {
                    SensorId = 3,
                    Value = 25,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagInteger
                {
                    SensorId = 4,
                    Value = 11,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagInteger
                {
                    SensorId = 5,
                    Value = 11,
                    TagType = TagDataTypeEnum.Integer
                }
            }
        }
    };

    public static ICollection<Tracker> GetTrackers()
    {
        var sensors = SensorRepositoryMock.GetSensors().ToList();
        return new List<Tracker>
        {
            new()
            {
                Id = 1,
                Name = "трекер GalileoSky",
                Description = "Описание 1",
                Imei = "12341",
                ExternalId = 111,
                StartDate = DateTime.MinValue,
                TrackerType = TrackerTypeEnum.GalileoSkyV50,
                SimNumber = "905518101010",
                Sensors = sensors.Where(s => s.TrackerId == 1).ToList()
            },
            new()
            {
                Id = 2,
                Name = "трекер Retranslator",
                Description = "Описание 2",
                Imei = "12342",
                ExternalId = 222,
                StartDate = DateTime.UnixEpoch,
                TrackerType = TrackerTypeEnum.Retranslator,
                SimNumber = "905518101020",
                Sensors = sensors.Where(s => s.TrackerId == 2).ToList()
            },
            new()
            {
                Id = 3,
                Name = "трекер WialonIPS",
                Description = "Описание 3",
                Imei = "12343",
                ExternalId = 333,
                StartDate = DateTime.MaxValue,
                TrackerType = TrackerTypeEnum.WialonIPS,
                SimNumber = "905518101030",
                Sensors = sensors.Where(s => s.TrackerId == 3).ToList()
            },
            new()
            {
                Id = 4,
                Name = "трекер WialonIPS",
                Description = "Описание 4",
                Imei = "64128256",
                ExternalId = 128,
                StartDate = DateTime.MaxValue,
                TrackerType = TrackerTypeEnum.WialonIPS,
                Sensors = new List<Sensor>
                {
                    new Sensor
                    {
                        Id = 3,
                        Name = "Датчик 1",
                        IsVisible = true,
                        UnitId = 1,
                        Unit = new Unit(1, "Килограмм", "кг")
                    },
                    new Sensor
                    {
                        Id = 4,
                        Name = "Датчик 2",
                        IsVisible = true,
                        UnitId = 2,
                        Unit = new Unit(2, "Метр", "м")
                    },
                    new Sensor
                    {
                        Id = 5,
                        Name = "Датчик 3",
                        IsVisible = false,
                        UnitId = 2,
                        Unit = new Unit(2, "Метр", "м")
                    }
                }
            }
        };
    }
}