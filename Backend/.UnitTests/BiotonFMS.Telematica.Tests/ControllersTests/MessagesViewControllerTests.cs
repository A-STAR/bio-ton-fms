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
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit.Abstractions;
using BioTonFMS.Telematica.Validation;
using BioTonFMS.Telematica.Dtos;
using System.Collections;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

/// <summary>
/// Тесты для <see cref="MessagesViewController"/>
/// </summary>
public class MessagesViewControllerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    private static DateTime _systemNow = DateTime.UtcNow;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public MessagesViewControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #region FindVehicles
    public static IEnumerable<object[]> FindVehicleCriterionData =>
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
            // поиск по IMEI трекера, должна быть найдена одна машина
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
            // поиск по несуществующему IMEI, машин не должно быть
            new object[]
            {
                "6412",
                Array.Empty<MessagesViewVehicleDto>()
            },
            // поиск по внешнему id трекера, должна быть найдена одна машина
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
            // фильтр поиска - пустая строка, должны быть найдены все машины с трекером
            new object[]
            {
                "",
                new[]
                {
                    new MessagesViewVehicleDto { Id = 1, Name = "Красная машина" },
                    new MessagesViewVehicleDto { Id = 3, Name = "Желтая машина" },
                    new MessagesViewVehicleDto { Id = 2, Name = "Синяя машина" },
                    new MessagesViewVehicleDto { Id = 4, Name = "Чёрный трактор" },
                    new MessagesViewVehicleDto { Id = 6, Name = "Фиолетовый трактор" },
                    new MessagesViewVehicleDto { Id = 7, Name = "Фиолетовый трактор 2" }
                }
            },
            // фильтр поиска не совпадает с названиями, машин не должно быть
            new object[]
            {
                "такой строки нет",
                Array.Empty<MessagesViewVehicleDto>()
            },
            // фильтр поиска - mull, должны быть найдены все машины с трекером
            new object[]
            {
                null!,
                new[]
                {
                    new MessagesViewVehicleDto { Id = 1, Name = "Красная машина" },
                    new MessagesViewVehicleDto { Id = 3, Name = "Желтая машина" },
                    new MessagesViewVehicleDto { Id = 2, Name = "Синяя машина" },
                    new MessagesViewVehicleDto { Id = 4, Name = "Чёрный трактор" },
                    new MessagesViewVehicleDto { Id = 6, Name = "Фиолетовый трактор" },
                    new MessagesViewVehicleDto { Id = 7, Name = "Фиолетовый трактор 2" }
                }
            }
        };

    [Theory, MemberData(nameof(FindVehicleCriterionData))]
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

    #endregion FindVehicles

    #region Statistics
    public static IEnumerable<object[]> StatisticsData =>
        new List<object[]>
        {
            //Запрос статистики по сообщениям, есть сообщения в периоде
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 1,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = SystemTime.UtcNow.AddHours(-100),
                    PeriodEnd = SystemTime.UtcNow.AddHours(100)
                },
                new OkObjectResult(new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 2,
                    TotalTime = 10,
                    AverageSpeed = 12.1,
                    MaxSpeed = 12.1
                })
            },
            //Запрос статистики по командам, есть команды в периоде
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 1,
                    ViewMessageType = ViewMessageTypeEnum.CommandMessage,
                    PeriodStart = SystemTime.UtcNow.AddHours(-100),
                    PeriodEnd = SystemTime.UtcNow.AddHours(100)
                },
                new OkObjectResult(new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 3,
                    TotalTime = 20
                })
            },
            //Несуществующая машина в запросе
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = -91001,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = SystemTime.UtcNow,
                    PeriodEnd = SystemTime.UtcNow
                },
                new NotFoundObjectResult("Трекер машины с таким id не существует")
            },
            //Машина без трекера в запросе
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 5,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = SystemTime.UtcNow,
                    PeriodEnd = SystemTime.UtcNow
                },
                new NotFoundObjectResult("Трекер машины с таким id не существует")
            },
            //Нет сообщений в периоде
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 2,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = SystemTime.UtcNow,
                    PeriodEnd = SystemTime.UtcNow
                },
                new OkObjectResult(new ViewMessageStatisticsDto())
            },
            //Сообщения без тегов
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 6,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = SystemTime.UtcNow.AddHours(-100),
                    PeriodEnd = SystemTime.UtcNow.AddHours(100)
                },
                new OkObjectResult(new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 2,
                    TotalTime = 10,
                    AverageSpeed = 50.1,
                    MaxSpeed = 50.1
                })
            },
            //Сообщения c тегами
            new object[]
            {
                new MessagesViewStatisticsRequest
                {
                    VehicleId = 7,
                    ViewMessageType = ViewMessageTypeEnum.DataMessage,
                    PeriodStart = SystemTime.UtcNow.AddHours(-100),
                    PeriodEnd = SystemTime.UtcNow.AddHours(100)
                },
                new OkObjectResult(new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 2,
                    TotalTime = 10,
                    AverageSpeed = 50.1,
                    MaxSpeed = 50.1,
                    Distance = 1,
                    Mileage = 14981
                })
            }
        };

    [Theory, MemberData(nameof(StatisticsData))]
    public void GetMessagesViewStatistics(MessagesViewStatisticsRequest request,
        ObjectResult expected)
    {
        SystemTime.Set(DateTime.UtcNow);
        _testOutputHelper.WriteLine("Request:\n" + JsonSerializer.Serialize(request, _jsonOptions));

        var result = GetController().GetMessagesViewStatistics(request);

        expected.StatusCode.Should().Be(result.As<ObjectResult>().StatusCode);

        expected.Value.Should().BeEquivalentTo(result.As<ObjectResult>().Value);
    }

    #endregion Statistics

    #region Tracks
    public static IEnumerable<object[]> TrackData =>
        new List<object[]>
        {
            //Машина с трекером и с сообщениями
            new object[]
            {
                new MessagesViewTrackRequest
                {
                    VehicleId = 1,
                    PeriodStart = SystemTime.UtcNow.AddHours(-100),
                    PeriodEnd = SystemTime.UtcNow.AddHours(100)
                },
                new OkObjectResult(new LocationsAndTracksResponse
                {
                    ViewBounds = new ViewBounds
                    {
                        UpperLeftLatitude = 49.782023,
                        UpperLeftLongitude = 52.456860999999996,
                        BottomRightLatitude = 42.082023,
                        BottomRightLongitude = 54.656861
                    },
                    Tracks = new List<LocationAndTrack>
                    {
                        new()
                        {
                            VehicleId = 1,
                            Latitude = 42.432023000000001,
                            Longitude = 54.556860999999998,
                            VehicleName = "Красная машина",
                            Track = new TrackPointInfo[]
                            {
                                new()
                                {
                                    MessageId = 1,
                                    Latitude = 49.432023,
                                    Longitude = 52.556861,
                                    Speed = null,
                                    NumberOfSatellites = 12,
                                    Time = SystemTime.WithNegativeDelta(TimeSpan.FromSeconds(40))
                                },
                                new()
                                {
                                    MessageId = 2,
                                    Latitude = 42.432023,
                                    Longitude = 54.556861,
                                    Speed = 12.1,
                                    NumberOfSatellites = 14,
                                    Time = SystemTime.WithNegativeDelta(TimeSpan.FromSeconds(30))
                                }
                            }
                        }
                    }
                })
            },
            //Несуществующая машина
            new object[]
            {
                new MessagesViewTrackRequest
                {
                    VehicleId = -13432,
                    PeriodStart = SystemTime.UtcNow.AddHours(-100),
                    PeriodEnd = SystemTime.UtcNow.AddHours(100)
                },
                new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
            },
            // Машина без трекера
            new object[]
            {
                new MessagesViewTrackRequest
                {
                    VehicleId = 5,
                    PeriodStart = SystemTime.UtcNow.AddHours(-100),
                    PeriodEnd = SystemTime.UtcNow.AddHours(100)
                },
                new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
            },
            //Машина с трекером без сообщений
            new object[]
            {
                new MessagesViewTrackRequest
                {
                    VehicleId = 2,
                    PeriodStart = SystemTime.UtcNow.AddHours(-100),
                    PeriodEnd = SystemTime.UtcNow.AddHours(100)
                },
                new OkObjectResult(new LocationsAndTracksResponse{Tracks = new List<LocationAndTrack>()})
            }
        };

    [Theory, MemberData(nameof(TrackData))]
    public void GetMessagesViewTrack(MessagesViewTrackRequest request, ObjectResult expected)
    {
        _testOutputHelper.WriteLine($"id: {request.VehicleId}; dates: {request.PeriodStart} - {request.PeriodEnd}");

        var result = GetController().GetMessagesViewTrack(request);

        expected.StatusCode.Should().Be(result.As<ObjectResult>().StatusCode);

        if (expected.StatusCode == 200)
        {
            var actual = result.As<OkObjectResult>().Value.As<LocationsAndTracksResponse>();

            actual.Should().BeEquivalentTo(expected.Value);

            if (actual.Tracks.IsNullOrEmpty()) return;
            
            actual.Tracks.Should().ContainSingle();
            var track = actual.Tracks.Single();

            var lastPoint = track.Track.OrderBy(x => x.Time).Last();
            lastPoint.Longitude.Should().Be(track.Longitude);
            lastPoint.Latitude.Should().Be(track.Latitude);
        }
        else
        {
            expected.Value.Should().BeEquivalentTo(result.As<ObjectResult>().Value);
        }
    }

    #endregion Tracks

    #region ParamsData
    public static IEnumerable<object[]> ViewMessagesTrackerParamsData =>
    new List<object[]>
    {
        // Несуществующая машина
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = -13432,
                PeriodStart = SystemTime.UtcNow.AddHours(-100),
                PeriodEnd = SystemTime.UtcNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.TrackerData,
                PageNum = 1,
                PageSize = 10
            },
            new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
        },
        // Машина без трекера
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 5,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.TrackerData,
                PageNum = 1,
                PageSize = 10
            },
            new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
        },
        //Машина с трекером без сообщений
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 2,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.TrackerData,
                PageNum = 1,
                PageSize = 10
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                TrackerDataMessages = new TrackerDataMessageDto[]{}, 
                Pagination = new Pagination
                {
                     Records = 0,
                     Total = 0,
                     PageIndex = 0
                } 
            })
        },
       // Машина с сообщениями в заданном диапазоне
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 1,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.TrackerData,
                PageNum = 1,
                PageSize = 10
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                TrackerDataMessages = new TrackerDataMessageDto[]{
                    new TrackerDataMessageDto
                    { 
                        Altitude = 97.0,
                        Id = 1,
                        Latitude = 49.432023,
                        Longitude = 52.556861,
                        Num = 1,
                        SatNumber = 12,
                        ServerDateTime = _systemNow - TimeSpan.FromSeconds(40),
                        TrackerDateTime = _systemNow - TimeSpan.FromSeconds(40),
                        Parameters = new TrackerParameter[] {
                            new TrackerParameter
                            {
                                LastValueDecimal = 1234.0,
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
                    },
                    new TrackerDataMessageDto
                    {
                        Altitude = 97.0,
                        Id = 2,
                        Latitude = 42.432023,
                        Longitude = 54.556861,
                        Num = 2,
                        SatNumber = 14,
                        Speed = 12.1,
                        ServerDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        TrackerDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        Parameters = new TrackerParameter[] {
                            new TrackerParameter
                            {
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
                                LastValueDecimal = 2134.0,
                                ParamName = "rs485_1"
                            }
                        }
                    },
                },
                Pagination = new Pagination
                {
                     Records = 2,
                     Total = 1,
                     PageIndex = 1
                }
            })
        },
       // Машина с сообщениями в заданном диапазоне, размер страницы - 1, вернуть вторую страницу
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 1,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.TrackerData,
                PageNum = 2,
                PageSize = 1
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                TrackerDataMessages = new TrackerDataMessageDto[]{
                    new TrackerDataMessageDto
                    {
                        Altitude = 97.0,
                        Id = 2,
                        Latitude = 42.432023,
                        Longitude = 54.556861,
                        Num = 2,
                        SatNumber = 14,
                        Speed = 12.1,
                        ServerDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        TrackerDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        Parameters = new TrackerParameter[] {
                            new TrackerParameter
                            {
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
                                LastValueDecimal = 2134.0,
                                ParamName = "rs485_1"
                            }
                        }
                    },
                },
                Pagination = new Pagination
                {
                     Records = 2,
                     Total = 2,
                     PageIndex = 2
                }
            })
        }
    };

    [Theory, MemberData(nameof(ViewMessagesTrackerParamsData))]
    public void GetMessagesViewMessages_ShouldReturnDataMessagesWithTrackerParamsData(MessagesViewMessagesRequest request, ObjectResult expected)
    {
        _testOutputHelper.WriteLine($"id: {request.VehicleId}; dates: {request.PeriodStart} - {request.PeriodEnd}");
        SystemTime.Set(_systemNow);

        var result = GetController().GetMessagesViewMessages(request);

        expected.StatusCode.Should().Be(result.As<ObjectResult>().StatusCode);

        if (expected.StatusCode == 200)
        {
            var actual = result.As<OkObjectResult>().Value.As<ViewMessageMessagesDto>();
            actual.Should().BeEquivalentTo(expected.Value);
        }
        else
        {
            expected.Value.Should().BeEquivalentTo(result.As<ObjectResult>().Value);
        }
    }

    #endregion ParamsData

    #region SensorData
    public static IEnumerable<object[]> ViewMessagesTrackerSensorData =>
    new List<object[]>
    {
        // в набор датчиков для теста входит один датчик с признаком IsVisible = false, он не должен попадать в результат
        // один из датчиков в тесте не имеет значения в сообщениях
        // Несуществующая машина
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = -13432,
                PeriodStart = SystemTime.UtcNow.AddHours(-100),
                PeriodEnd = SystemTime.UtcNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.SensorData,
                PageNum = 1,
                PageSize = 10
            },
            new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
        },
        // Машина без трекера
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 5,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.SensorData,
                PageNum = 1,
                PageSize = 10
            },
            new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
        },
        //Машина с трекером без сообщений
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 2,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.SensorData,
                PageNum = 1,
                PageSize = 10
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                SensorDataMessages = new SensorDataMessageDto[]{},
                Pagination = new Pagination
                {
                     Records = 0,
                     Total = 0,
                     PageIndex = 0
                }
            })
        },
       // Машина с сообщениями в заданном диапазоне
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 1,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.SensorData,
                PageNum = 1,
                PageSize = 10
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                SensorDataMessages = new SensorDataMessageDto[]{
                    new SensorDataMessageDto
                    {
                        Altitude = 97.0,
                        Id = 1,
                        Latitude = 49.432023,
                        Longitude = 52.556861,
                        Num = 1,
                        SatNumber = 12,
                        ServerDateTime = _systemNow - TimeSpan.FromSeconds(40),
                        TrackerDateTime = _systemNow - TimeSpan.FromSeconds(40),
                        Sensors = new TrackerSensorDto[] {
                            new TrackerSensorDto
                            {
                                Name = "a",
                                Value = "1234",
                                Unit = "м"
                            },
                            new TrackerSensorDto
                            {
                                Name = "b",
                                Value = "1234.12",
                                Unit = "м"
                            },
                            new TrackerSensorDto
                            {
                                Name = "e",
                                Unit = "м"
                            }
                        }
                    },
                    new SensorDataMessageDto
                    {
                        Altitude = 97.0,
                        Id = 2,
                        Latitude = 42.432023,
                        Longitude = 54.556861,
                        Num = 2,
                        SatNumber = 14,
                        Speed = 12.1,
                        ServerDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        TrackerDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        Sensors = new TrackerSensorDto[] {
                            new TrackerSensorDto
                            {
                                Name = "a",
                                Value = "1234",
                                Unit = "м"
                            },
                            new TrackerSensorDto
                            {
                                Name = "b",
                                Value = "1234.12",
                                Unit = "м"
                            },
                            new TrackerSensorDto
                            {
                                Name = "e",
                                Unit = "м"
                            }
                        }
                    },
                },
                Pagination = new Pagination
                {
                     Records = 2,
                     Total = 1,
                     PageIndex = 1
                }
            })
        },
        // Машина с сообщениями в заданном диапазоне, размер страницы - 1, вернуть вторую страницу
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 1,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.DataMessage,
                ParameterType = ParameterTypeEnum.SensorData,
                PageNum = 2,
                PageSize = 1
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                SensorDataMessages = new SensorDataMessageDto[]{
                    new SensorDataMessageDto
                    {
                        Altitude = 97.0,
                        Id = 2,
                        Latitude = 42.432023,
                        Longitude = 54.556861,
                        Num = 2,
                        SatNumber = 14,
                        Speed = 12.1,
                        ServerDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        TrackerDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        Sensors = new TrackerSensorDto[] {
                            new TrackerSensorDto
                            {
                                Name = "a",
                                Value = "1234",
                                Unit = "м"
                            },
                            new TrackerSensorDto
                            {
                                Name = "b",
                                Value = "1234.12",
                                Unit = "м"
                            },
                            new TrackerSensorDto
                            {
                                Name = "e",
                                Unit = "м"
                            }
                        }
                    },
                },
                Pagination = new Pagination
                {
                     Records = 2,
                     Total = 2,
                     PageIndex = 2
                }
            })
        }
    };

    [Theory, MemberData(nameof(ViewMessagesTrackerSensorData))]
    public void GetMessagesViewMessages_ShouldReturnDataMessagesWithTrackerSensorData(MessagesViewMessagesRequest request, ObjectResult expected)
    {
        _testOutputHelper.WriteLine($"id: {request.VehicleId}; dates: {request.PeriodStart} - {request.PeriodEnd}");
        SystemTime.Set(_systemNow);

        var result = GetController(GetTrackersForSensorData(), null, MessagesForSensorData).GetMessagesViewMessages(request);

        expected.StatusCode.Should().Be(result.As<ObjectResult>().StatusCode);

        if (expected.StatusCode == 200)
        {
            var actual = result.As<OkObjectResult>().Value.As<ViewMessageMessagesDto>();
            actual.Should().BeEquivalentTo(expected.Value);
        }
        else
        {
            expected.Value.Should().BeEquivalentTo(result.As<ObjectResult>().Value);
        }
    }
    #endregion SensorData

    #region CommandData
    public static IEnumerable<object[]> ViewMessagesCommandData =>
    new List<object[]>
    {
        // Несуществующая машина
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = -13432,
                PeriodStart = SystemTime.UtcNow.AddHours(-100),
                PeriodEnd = SystemTime.UtcNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.CommandMessage,
                PageNum = 1,
                PageSize = 10
            },
            new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
        },
        // Машина без трекера
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 5,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.CommandMessage,
                PageNum = 1,
                PageSize = 10
            },
            new NotFoundObjectResult("Машина с таким id не существует, либо к ней не привязан трекер")
        },
        //Машина с трекером без команд
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 2,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.CommandMessage,
                PageNum = 1,
                PageSize = 10
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                CommandMessages = new CommandMessageDto[]{},
                Pagination = new Pagination
                {
                     Records = 0,
                     Total = 0,
                     PageIndex = 0
                }
            })
        },
       // Машина с командами в заданном диапазоне
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 1,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.CommandMessage,
                PageNum = 1,
                PageSize = 10
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                CommandMessages = new CommandMessageDto[]{
                    new CommandMessageDto
                    {
                        Id = 1,
                        Num = 1,
                        CommandDateTime = _systemNow - TimeSpan.FromSeconds(40),
                        CommandText = "IMEI",
                        CommandResponseText = ""
                    },
                    new CommandMessageDto
                    {
                        Id = 2,
                        Num = 2,
                        CommandDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        CommandText = "TEST",
                        CommandResponseText = "TEST RESPONSE",
                        ExecutionTime = _systemNow - TimeSpan.FromSeconds(25)
                    },
                },
                Pagination = new Pagination
                {
                     Records = 2,
                     Total = 1,
                     PageIndex = 1
                }
            })
        },
        // Машина с командами в заданном диапазоне, размер страницы - 1, вернуть вторую страницу
        new object[]
        {
            new MessagesViewMessagesRequest
            {
                VehicleId = 1,
                PeriodStart = _systemNow.AddHours(-100),
                PeriodEnd = _systemNow.AddHours(100),
                ViewMessageType = ViewMessageTypeEnum.CommandMessage,
                PageNum = 2,
                PageSize = 1
            },
            new OkObjectResult(new ViewMessageMessagesDto
            {
                CommandMessages = new CommandMessageDto[]{
                    new CommandMessageDto
                    {
                        Id = 2,
                        Num = 2,
                        CommandDateTime = _systemNow - TimeSpan.FromSeconds(30),
                        CommandText = "TEST",
                        CommandResponseText = "TEST RESPONSE",
                        ExecutionTime = _systemNow - TimeSpan.FromSeconds(25)
                    },
                },
                Pagination = new Pagination
                {
                     Records = 2,
                     Total = 2,
                     PageIndex = 2
                }
            })
        },
    };

    [Theory, MemberData(nameof(ViewMessagesCommandData))]
    public void GetMessagesViewMessages_ShouldReturnComandMessages(MessagesViewMessagesRequest request, ObjectResult expected)
    {
        _testOutputHelper.WriteLine($"id: {request.VehicleId}; dates: {request.PeriodStart} - {request.PeriodEnd}");
        SystemTime.Set(_systemNow);

        var result = GetController(GetTrackersForSensorData(), null, MessagesForSensorData, Commands).GetMessagesViewMessages(request);

        expected.StatusCode.Should().Be(result.As<ObjectResult>().StatusCode);

        if (expected.StatusCode == 200)
        {
            var actual = result.As<OkObjectResult>().Value.As<ViewMessageMessagesDto>();
            actual.Should().BeEquivalentTo(expected.Value);
        }
        else
        {
            expected.Value.Should().BeEquivalentTo(result.As<ObjectResult>().Value);
        }
    }
    #endregion CommandData

    #region DeleteMessages
    [Fact]
    public void DeleteMessages_ShouldReturnNotFound_IfIdListIsEmpty()
    {
        long[] messageIds = new long[] { };

        var result = GetController().DeleteMessages(messageIds);

        result.As<ObjectResult>().StatusCode.Should().Be(404);
        result.As<ObjectResult>().Value.Should().Be("Пустой список идентификаторов для удаления");
    }

    [Fact]
    public void DeleteMessages_ShouldReturnNotFound_IfOneIdFromListNotExists()
    {
        long[] messageIds = new long[] { 1, 2, -100 };

        var result = GetController().DeleteMessages(messageIds);

        result.As<ObjectResult>().StatusCode.Should().Be(404);
        result.As<ObjectResult>().Value.Should().BeEquivalentTo(new ServiceErrorResult("Для некоторых идентификаторов из списка не найдены сообщения"));
    }

    [Fact]
    public void DeleteMessages_ShouldDeleteMessage_IfOneCorrectIdInList()
    {
        long[] messageIds = new long[] { 1 };

        var messageRepository = TrackerMessageRepositoryMock.GetStub();
        messageRepository[1].Should().NotBeNull();
        var result = GetController(messageRepository).DeleteMessages(messageIds);

        result.As<OkResult>().StatusCode.Should().Be(200);
        messageRepository[1].Should().BeNull();
        messageRepository[2].Should().NotBeNull();
    }

    [Fact]
    public void DeleteMessages_ShouldDeleteMessages_IfTwoCorrectIdInList()
    {
        long[] messageIds = new long[] { 1, 2 };

        var messageRepository = TrackerMessageRepositoryMock.GetStub();
        messageRepository[1].Should().NotBeNull();
        messageRepository[2].Should().NotBeNull();
        var result = GetController(messageRepository).DeleteMessages(messageIds);

        result.As<OkResult>().StatusCode.Should().Be(200);
        messageRepository[1].Should().BeNull();
        messageRepository[2].Should().BeNull();
    }
    #endregion DeleteMessages

    #region DeleteCommandMessages
    [Fact]
    public void DeleteCommandMessages_ShouldReturnNotFound_IfIdListIsEmpty()
    {
        int[] messageIds = new int[] { };

        var result = GetController().DeleteCommandMessages(messageIds);

        result.As<ObjectResult>().StatusCode.Should().Be(404);
        result.As<ObjectResult>().Value.Should().Be("Пустой список идентификаторов для удаления");
    }

    [Fact]
    public void DeleteCommandMessages_ShouldReturnNotFound_IfOneIdFromListNotExists()
    {
        int[] messageIds = new int[] { 1, 2, -100 };

        var result = GetController().DeleteCommandMessages(messageIds);

        result.As<ObjectResult>().StatusCode.Should().Be(404);
        result.As<ObjectResult>().Value.Should().BeEquivalentTo(new ServiceErrorResult("Для некоторых идентификаторов из списка не найдены сообщения"));
    }

    [Fact]
    public void DeleteCommandMessages_ShouldDeleteMessage_IfOneCorrectIdInList()
    {
        int[] messageIds = new int[] { 1 };

        var commandRepository = TrackerCommandRepositoryMock.GetStub();
        commandRepository[1].Should().NotBeNull();
        var result = GetController(commandRepository).DeleteCommandMessages(messageIds);

        result.As<OkResult>().StatusCode.Should().Be(200);
        commandRepository[1].Should().BeNull();
        commandRepository[2].Should().NotBeNull();
    }

    [Fact]
    public void DeleteCommandMessages_ShouldDeleteMessages_IfTwoCorrectIdInList()
    {
        int[] messageIds = new int[] { 1, 2 };

        var commandRepository = TrackerCommandRepositoryMock.GetStub();

        commandRepository[1].Should().NotBeNull();
        commandRepository[2].Should().NotBeNull();
        var result = GetController(commandRepository).DeleteCommandMessages(messageIds);

        result.As<OkResult>().StatusCode.Should().Be(200);
        commandRepository[1].Should().BeNull();
        commandRepository[2].Should().BeNull();
    }
    #endregion DeleteCommandMessages

    #region Controller & Repository data
    private static MessagesViewController GetController(
        ICollection<Tracker>? trackers = null,
        ICollection<Vehicle>? vehicles = null,
        ICollection<TrackerMessage>? messages = null,
        ICollection<TrackerCommand>? commands = null)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MessagesViewMappingProfile())));
        var logger = new Mock<ILogger<MessagesViewController>>().Object;
        var trackerRepository = TrackerRepositoryMock.GetStub(trackers);
        var vehicleRepository = VehicleRepositoryMock.GetStub(vehicles);
        ITrackerCommandRepository commandRepository = TrackerCommandRepositoryMock.GetStub(commands);
        var messageRepository = TrackerMessageRepositoryMock.GetStub(messages, trackerRepository);
        var messagesViewMessagesRequestValidator = new MessagesViewMessagesRequestValidator();

        return new MessagesViewController(logger, mapper, messagesViewMessagesRequestValidator, vehicleRepository, commandRepository, messageRepository);
    }

    private static MessagesViewController GetController(ITrackerMessageRepository messageRepository)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MessagesViewMappingProfile())));
        var logger = new Mock<ILogger<MessagesViewController>>().Object;
        var trackerRepository = TrackerRepositoryMock.GetStub();
        var vehicleRepository = VehicleRepositoryMock.GetStub();
        var commandRepository = TrackerCommandRepositoryMock.GetStub();
        var messagesViewMessagesRequestValidator = new MessagesViewMessagesRequestValidator();

        return new MessagesViewController(logger, mapper, messagesViewMessagesRequestValidator, vehicleRepository, commandRepository, messageRepository);
    }

    private static MessagesViewController GetController(ITrackerCommandRepository commandRepository)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MessagesViewMappingProfile())));
        var logger = new Mock<ILogger<MessagesViewController>>().Object;
        var trackerRepository = TrackerRepositoryMock.GetStub();
        var vehicleRepository = VehicleRepositoryMock.GetStub();
        var messageRepository = TrackerMessageRepositoryMock.GetStub(null, trackerRepository);
        var messagesViewMessagesRequestValidator = new MessagesViewMessagesRequestValidator();

        return new MessagesViewController(logger, mapper, messagesViewMessagesRequestValidator, vehicleRepository, commandRepository, messageRepository);
    }

    private static ICollection<Tracker> GetTrackersForSensorData()
    {
        var sensors = GetSensorsForSensorData().ToList();
        return new List<Tracker>
        {
            new()
            {
                Id = 1,
                Name = "трекер GalileoSky",
                Description = "Описание 1",
                Imei = "12341",
                ExternalId = 2552,
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
                ExternalId = 15,
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
                Id = 121,
                Name = "трекер WialonIPS",
                Description = "Описание 121",
                Imei = "123435",
                ExternalId = 444,
                StartDate = DateTime.MaxValue,
                TrackerType = TrackerTypeEnum.WialonIPS,
                SimNumber = "905518101030",
                Sensors = sensors.Where(s => s.TrackerId == 3).ToList()
            }
        };
    }

    public static IEnumerable<Sensor> GetSensorsForSensorData() => new List<Sensor>
    {
        new()
        {
            Id = 1, TrackerId = 1, Name = "a", Formula = "b", IsVisible = true, Unit = new Unit(1, "м", "м")
        },
        new()
        {
            Id = 2, TrackerId = 1, Name = "b", Formula = "const1", IsVisible = true, Unit = new Unit(1, "м", "м")
        },
        new()
        {
            Id = 3, TrackerId = 1, Name = "c", Formula = "const2", IsVisible = false, Unit = new Unit(1, "м", "м")
        },
        new()
        {
            Id = 4, TrackerId = 1, Name = "e", Formula = "const3", IsVisible = true, Unit = new Unit(1, "м", "м")
        }
    };

    public static TrackerMessage[] MessagesForSensorData => new TrackerMessage[]
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
                },
                new MessageTagInteger
                {
                    SensorId = 1,
                    Value = 1234,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagDouble
                {
                    SensorId = 2,
                    Value = 1234.12,
                    TagType = TagDataTypeEnum.Double
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
                },
                new MessageTagInteger
                {
                    SensorId = 1,
                    Value = 1234,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagDouble
                {
                    SensorId = 2,
                    Value = 1234.12,
                    TagType = TagDataTypeEnum.Double
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
        }
    };

    public static List<TrackerCommand> Commands => new()
    {
        new TrackerCommand
        {
            Id = 1,
            Tracker = new Tracker{ExternalId = 2552},
            SentDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
            CommandText = "IMEI"
        },
        new TrackerCommand
        {
            Id = 2,
            Tracker = new Tracker{ExternalId = 2552},
            SentDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            CommandText = "TEST",
            ResponseText = "TEST RESPONSE",
            ResponseDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(25),
        },
    };

    #endregion Controller & Repository data
}