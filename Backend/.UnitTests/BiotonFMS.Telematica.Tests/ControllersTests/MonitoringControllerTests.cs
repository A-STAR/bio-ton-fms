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
                        NumberOfSatellites = 12,
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
                        LastMessageTime = DateTime.UtcNow,
                        NumberOfSatellites = 12
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
                        LastMessageTime = DateTime.UtcNow,
                        NumberOfSatellites = 12
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
                        LastMessageTime = DateTime.UtcNow,
                        NumberOfSatellites = 12
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
                    }
                }
            }
        };
    
    [Theory, MemberData(nameof(CriterionData))]
    public void FindVehicles(string? criterion, MonitoringVehicleDto[] expected)
    {
        _testOutputHelper.WriteLine("Criterion = \"" + criterion + "\"");

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

    private static MonitoringController GetController()
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MonitoringMappingProfile())));
        var logger = new Mock<ILogger<MonitoringController>>().Object;
        var vehicleRepository = VehicleRepositoryMock.GetStub();
        var trackerMessageRepository = TrackerMessageRepositoryMock.GetStub();
        var options = Options.Create(new TrackerOptions { TrackerAddressValidMinutes = 60 });
        var tagsRepository = TrackerTagRepositoryMock.GetStub();
        var trackerRepository = TrackerRepositoryMock.GetStub();

        return new MonitoringController(mapper, logger, vehicleRepository,
            trackerMessageRepository, options, tagsRepository, trackerRepository);
    }
}