using System.Globalization;
using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos;
using BioTonFMS.Telematica.Mapping;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.Telematica.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

public class SensorControllerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SensorControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ru");
    }

    #region GetSensor
    [Fact]
    public void GetSensor_SensorExists_ShouldReturn200Status()
    {
        var controller = GetController();

        var result = (ObjectResult)controller.GetSensor(SensorRepositoryMock.ExistentSensorId);

        result.Should().BeOfType<OkObjectResult>();
        result.StatusCode.Should().Be(200);

        var content = result.Value.As<SensorDto>();
        content.Should().NotBeNull();
    }

    [Fact]
    public void GetSensor_SensorNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var result = (ObjectResult)controller.GetSensor(SensorRepositoryMock.NonExistentSensorId);

        result.Should().BeOfType<NotFoundObjectResult>();
        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        content.Messages.Should().Contain(m => m.ToLower().Contains("не найден") && m.ToLower().Contains("датчик"));
    }
    #endregion

    #region Create/update constraints
    public static IEnumerable<object[]> ConstraintViolationTestData =>
        new[]
        {
            new object[]
            {
                "[POSITIVE] Tracker Does Not Exist",
                new Sensor()
                {
                    Name = "Sensor", TrackerId = TrackerRepositoryMock.NonExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                    SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, ValidatorId = SensorRepositoryMock.ExistentSensorId
                },
                "TrackerId",
                new[]
                {
                    "*Трекер* не существует*"
                }
            },
            new object[]
            {
                "[POSITIVE] Unit Does Not Exist",
                new Sensor()
                {
                    Name = "Sensor", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.NonExistentUnitId,
                    SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, ValidatorId = SensorRepositoryMock.ExistentSensorId
                },
                "UnitId",
                new[]
                {
                    "*Единица измерения * не существует*"
                }
            },
            new object[]
            {
                "[POSITIVE] Validator Does Not Exist",
                new Sensor()
                {
                    Name = "Sensor", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                    SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, ValidatorId = SensorRepositoryMock.NonExistentSensorId
                },
                "ValidatorId",
                new[]
                {
                    "*Датчик * не существует*"
                }
            },
            new object[]
            {
                "[POSITIVE] Sensor Type Does Not Exist",
                new Sensor()
                {
                    Name = "Sensor", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                    SensorTypeId = SensorTypeRepositoryMock.NonExistentSensorTypeId, ValidatorId = SensorRepositoryMock.ExistentSensorId
                },
                "SensorTypeId",
                new[]
                {
                    "*Тип датчиков * не существует*"
                }
            },
            new object[]
            {
                "[POSITIVE] Name is too short",
                new Sensor()
                {
                    Name = "", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                    SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, ValidatorId = SensorRepositoryMock.ExistentSensorId
                },
                "Name",
                new[]
                {
                    "*должно быть заполнено*", "*должно быть длиной*"
                }
            },
            new object[]
            {
                "[POSITIVE] Name is too long",
                new Sensor()
                {
                    Name = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901",
                    TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                    SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, ValidatorId = SensorRepositoryMock.ExistentSensorId
                },
                "Name",
                new[]
                {
                    "*должно быть длиной * символов*"
                }
            },
            new object[]
            {
                "[NEGATIVE] Description is empty",
                new Sensor()
                {
                    Name = "Sensor", Description = "", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                    SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, ValidatorId = SensorRepositoryMock.ExistentSensorId
                },
                "Description",
                Array.Empty<string>()
            },
            new object[]
            {
                "[NEGATIVE] Description is null",
                new Sensor()
                {
                    Name = "Sensor", Description = null!, TrackerId = TrackerRepositoryMock.ExistentTrackerId,
                    UnitId = UnitRepositoryMock.ExistentUnitId, SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId,
                    ValidatorId = SensorRepositoryMock.ExistentSensorId
                },
                "Description",
                Array.Empty<string>()
            },
            new object[]
            {
                "[POSITIVE] Description is too long",
                new Sensor()
                {
                    Name = "Sensor", Description = new String('a', 501), TrackerId = TrackerRepositoryMock.ExistentTrackerId,
                    UnitId = UnitRepositoryMock.ExistentUnitId, SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId,
                    ValidatorId = SensorRepositoryMock.ExistentSensorId
                },
                "Description",
                new[]
                {
                    "*должно быть длиной * символов*"
                }
            }
        };

    [Theory, MemberData(nameof(ConstraintViolationTestData))]
    public void UpdateSensor_SensorWithConstraintViolation_ThrowsArgumentException(string testName, Sensor sensor, string wrongField,
        string[] errors)
    {
        _testOutputHelper.WriteLine(testName);

        var controller = GetController();

        var mapperConfiguration = new MapperConfiguration(x => x.CreateMap<Sensor, UpdateSensorDto>());
        var mapper = mapperConfiguration.CreateMapper();
        var sensorDto = mapper.Map<Sensor, UpdateSensorDto>(sensor);

        var result = controller.UpdateSensor(SensorRepositoryMock.ExistentSensorId, sensorDto);
        result.Should().NotBeNull();

        if (errors.IsNullOrEmpty())
        {
            result.Should().BeOfType<OkResult>();
            ((OkResult)result).StatusCode.Should().Be(200);
        }
        else
        {
            result.Should().BeOfType<BadRequestObjectResult>();
            var badResult = ((BadRequestObjectResult)result);
            badResult.StatusCode.Should().Be(400);

            var content = badResult.Value.As<ValidationProblemDetails>();
            content.Should().NotBeNull();
            content.Errors.Should().HaveCount(1);
            content.Errors.Should().ContainKey(wrongField);
            foreach (var errorPattern in errors)
            {
                content.Errors[wrongField].Should().ContainMatch(errorPattern);
            }
        }
    }

    [Theory, MemberData(nameof(ConstraintViolationTestData))]
    public void AddSensor_SensorWithConstraintViolation_ThrowsArgumentException(string testName, Sensor sensor, string wrongField,
        string[] errors)
    {
        _testOutputHelper.WriteLine(testName);

        var controller = GetController();

        var mapperConfiguration = new MapperConfiguration(x => x.CreateMap<Sensor, CreateSensorDto>());
        var mapper = mapperConfiguration.CreateMapper();
        var sensorDto = mapper.Map<Sensor, CreateSensorDto>(sensor);

        var result = (ObjectResult)controller.AddSensor(sensorDto);

        if (errors.IsNullOrEmpty())
        {
            result.Should().BeOfType<OkObjectResult>();
            result.StatusCode.Should().Be(200);
        }
        else
        {
            result.Should().BeOfType<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);

            var content = result.Value.As<ValidationProblemDetails>();
            content.Should().NotBeNull();
            content.Errors.Should().HaveCount(1);
            content.Errors.Should().ContainKey(wrongField);
            foreach (var errorPattern in errors)
            {
                content.Errors[wrongField].Should().ContainMatch(errorPattern);
            }
        }
    }
    #endregion
    

    [Fact]
    public void AddSensor_SensorTypeWithDataTypeConstraint_SetsDataType()
    {
        var controller = GetController();

        var sensorDto = new CreateSensorDto()
        {
            Name = "a",
            TrackerId = TrackerRepositoryMock.ExistentTrackerId,
            Description = "",
            ValidatorId = null,
            SensorTypeId = SensorTypeRepositoryMock.SensorTypeWithBooleanDataTypeId,
            UnitId = UnitRepositoryMock.ExistentUnitId,
            Formula = "a",
            DataType = SensorDataTypeEnum.Number,
            FuelUse = 1,
            ValidationType = null,
            UseLastReceived = false
        };

        var result = controller.AddSensor(sensorDto);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);

        Assert.Equal(SensorDataTypeEnum.Boolean, SensorRepositoryMock.LastAddArgument!.DataType);
    }
    
    [Fact]
    public void AddSensor_SensorTypeWithUnitConstraint_SetsUnit()
    {
        var controller = GetController();

        var sensorDto = new CreateSensorDto()
        {
            Name = "a",
            TrackerId = TrackerRepositoryMock.ExistentTrackerId,
            Description = "",
            ValidatorId = null,
            SensorTypeId = SensorTypeRepositoryMock.SensorTypeWithSecondUnitId,
            UnitId = UnitRepositoryMock.MeterUnitId,
            Formula = "a",
            DataType = SensorDataTypeEnum.Number,
            FuelUse = 1,
            ValidationType = null,
            UseLastReceived = false
        };

        var result = controller.AddSensor(sensorDto);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);

        Assert.Equal(UnitRepositoryMock.SecondUnitId, SensorRepositoryMock.LastAddArgument!.UnitId);
    }

    [Fact]
    public void UpdateSensor_SensorTypeWithDataTypeConstraint_SetsDataType()
    {
        var controller = GetController();

        var sensorDto = new UpdateSensorDto()
        {
            Name = "a",
            TrackerId = TrackerRepositoryMock.ExistentTrackerId,
            Description = "",
            ValidatorId = null,
            SensorTypeId = SensorTypeRepositoryMock.SensorTypeWithBooleanDataTypeId,
            UnitId = UnitRepositoryMock.ExistentUnitId,
            Formula = "a",
            DataType = SensorDataTypeEnum.Number,
            FuelUse = 1,
            ValidationType = null,
            UseLastReceived = false
        };
        
        var result = controller.UpdateSensor(1, sensorDto);

        result.Should().BeOfType<OkResult>();
        var okResult = result as OkResult;
        okResult!.StatusCode.Should().Be(200);

        Assert.Equal(SensorDataTypeEnum.Boolean, SensorRepositoryMock.LastUpdateArgument!.DataType);
    }
    
    [Fact]
    public void UpdateSensor_SensorTypeWithUnitConstraint_SetsUnit()
    {
        var controller = GetController();

        var sensorDto = new UpdateSensorDto()
        {
            Name = "a",
            TrackerId = TrackerRepositoryMock.ExistentTrackerId,
            Description = "",
            ValidatorId = null,
            SensorTypeId = SensorTypeRepositoryMock.SensorTypeWithSecondUnitId,
            UnitId = UnitRepositoryMock.MeterUnitId,
            Formula = "a",
            DataType = SensorDataTypeEnum.Number,
            FuelUse = 1,
            ValidationType = null,
            UseLastReceived = false
        };

        var result = controller.UpdateSensor(1, sensorDto);

        result.Should().BeOfType<OkResult>();
        var okResult = result as OkResult;
        okResult!.StatusCode.Should().Be(200);

        Assert.Equal(2, SensorRepositoryMock.LastUpdateArgument!.UnitId);
    }

    private static SensorController GetController()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new SensorMappingProfile());
        });
        IMapper mapper = new Mapper(mapperConfig);

        return new SensorController(SensorRepositoryMock.GetStub(), SensorTypeRepositoryMock.GetStub(), mapper,
            new UpdateSensorDtoValidator(), new CreateSensorDtoValidator(),
            new SensorValidator(TrackerRepositoryMock.GetStub(), UnitRepositoryMock.GetStub(), SensorRepositoryMock.GetStub(),
            SensorTypeRepositoryMock.GetStub()), new SensorsRequestValidator());
    }

}
