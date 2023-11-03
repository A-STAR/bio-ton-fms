using AutoMapper;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos.MessagesView;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Mapping;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

/// <summary>
/// Тесты для <see cref="MessagesViewController"/>
/// </summary>
public class MessagesViewControllerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

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
                new []
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
                new []
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
                new []
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
                new []
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
                new []
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
                new []
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

        var result = GetController(MonitoringVehicles).FindVehicles(criterion);

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


    private static MessagesViewController GetController(
        ICollection<Vehicle>? vehicles = null,
        ICollection<TrackerMessage>? messages = null,
        ICollection<Tracker>? trackers = null)
    {
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MessagesViewMappingProfile())));
        var logger = new Mock<ILogger<MessagesViewController>>().Object;
        var vehicleRepository = VehicleRepositoryMock.GetStub(vehicles);
        var messageRepository = TrackerMessageRepositoryMock.GetStub();

        return new MessagesViewController(mapper, logger, vehicleRepository, messageRepository);
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


}