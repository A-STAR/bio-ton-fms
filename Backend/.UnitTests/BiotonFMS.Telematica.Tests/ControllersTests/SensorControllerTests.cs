using System.Globalization;
using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos;
using BioTonFMS.Telematica.Mapping;
using BiotonFMS.Telematica.Tests.Mocks;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using BioTonFMS.Telematica.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

public class SensorControllerTests
{
    public SensorControllerTests()
    {
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
    [Fact]
    public void UpdateSensor_SensorWithConstraintViolation_ReturnsValidationResults()
    {
        var controller = GetController();

        var sensorDto = new UpdateSensorDto()
        {
            Name = "", UnitId = UnitRepositoryMock.ExistentUnitId, Formula = "const1", TrackerId = TrackerRepositoryMock.ExistentTrackerId,
            FuelUse = 1, SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId
        };

        var result = controller.UpdateSensor(SensorRepositoryMock.ExistentSensorId, sensorDto);
        result.Should().NotBeNull();

        result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = ((BadRequestObjectResult)result);
        badResult.StatusCode.Should().Be(400);

        var content = badResult.Value.As<ValidationProblemDetails>();
        content.Should().NotBeNull();
        content.Errors.Should().HaveCount(1);
        content.Errors.Should().ContainKey(nameof(Sensor.Name));
        content.Errors[nameof(Sensor.Name)].Should().ContainMatch("*должно быть длиной*");
    }

    [Fact]
    public void AddSensor_SensorWithConstraintViolation_ReturnsValidationResults()
    {
        var controller = GetController();

        var sensorDto = new CreateSensorDto()
        {
            Name = "", UnitId = UnitRepositoryMock.ExistentUnitId, Formula = "const1", TrackerId = TrackerRepositoryMock.ExistentTrackerId,
            FuelUse = 1, SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId
        };

        var result = (ObjectResult)controller.AddSensor(sensorDto);
        result.Should().NotBeNull();

        result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = ((BadRequestObjectResult)result);
        badResult.StatusCode.Should().Be(400);

        var content = badResult.Value.As<ValidationProblemDetails>();
        content.Should().NotBeNull();
        content.Errors.Should().HaveCount(1);
        content.Errors.Should().ContainKey(nameof(Sensor.Name));
        content.Errors[nameof(Sensor.Name)].Should().ContainMatch("*должно быть длиной*");
    }
    #endregion


    [Fact]
    public void AddSensor_ValidSensor_Ok()
    {
        var controller = GetController();

        var sensorDto = new CreateSensorDto()
        {
            Name = "c", TrackerId = TrackerRepositoryMock.ExistentTrackerId, Description = "description", Formula = "const1",
            ValidatorId = SensorRepositoryMock.ExistentSensorId, ValidationType = ValidationTypeEnum.LogicalAnd,
            SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, UnitId = UnitRepositoryMock.MeterUnitId,
            DataType = SensorDataTypeEnum.Number, FuelUse = 1, UseLastReceived = false
        };

        var result = controller.AddSensor(sensorDto);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);

        SensorRepositoryMock.LastAddArgument.Should().NotBeNull();
        if (SensorRepositoryMock.LastAddArgument is null) return;
        SensorRepositoryMock.LastAddArgument.Name.Should().Be("c");
        SensorRepositoryMock.LastAddArgument.TrackerId.Should().Be(TrackerRepositoryMock.ExistentTrackerId);
        SensorRepositoryMock.LastAddArgument.Description.Should().Be("description");
        SensorRepositoryMock.LastAddArgument.Formula.Should().Be("const1");
        SensorRepositoryMock.LastAddArgument.ValidatorId.Should().Be(SensorRepositoryMock.ExistentSensorId);
        SensorRepositoryMock.LastAddArgument.ValidationType.Should().Be(ValidationTypeEnum.LogicalAnd);
        SensorRepositoryMock.LastAddArgument.SensorTypeId.Should().Be(SensorTypeRepositoryMock.ExistentSensorTypeId);
        SensorRepositoryMock.LastAddArgument.UnitId.Should().Be(UnitRepositoryMock.MeterUnitId);
        SensorRepositoryMock.LastAddArgument.DataType.Should().Be(SensorDataTypeEnum.Number);
        SensorRepositoryMock.LastAddArgument.FuelUse.Should().Be(1);
        SensorRepositoryMock.LastAddArgument.UseLastReceived.Should().Be(false);
    }

    [Fact]
    public void AddSensor_SensorTypeWithDataTypeConstraint_SetsDataType()
    {
        var controller = GetController();

        var sensorDto = new CreateSensorDto()
        {
            Name = "c", TrackerId = TrackerRepositoryMock.ExistentTrackerId, Description = "", ValidatorId = null,
            SensorTypeId = SensorTypeRepositoryMock.SensorTypeWithBooleanDataTypeId, UnitId = UnitRepositoryMock.ExistentUnitId,
            Formula = "const1", DataType = SensorDataTypeEnum.Number, FuelUse = 1, ValidationType = null,
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
            Name = "c", TrackerId = TrackerRepositoryMock.ExistentTrackerId, Description = "", ValidatorId = null,
            SensorTypeId = SensorTypeRepositoryMock.SensorTypeWithSecondUnitId, UnitId = UnitRepositoryMock.MeterUnitId, Formula = "const1",
            DataType = SensorDataTypeEnum.Number, FuelUse = 1, ValidationType = null, UseLastReceived = false
        };

        var result = controller.AddSensor(sensorDto);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);

        Assert.Equal(UnitRepositoryMock.SecondUnitId, SensorRepositoryMock.LastAddArgument!.UnitId);
    }

    [Fact]
    public void UpdateSensor_ValidSensor_Ok()
    {
        var controller = GetController();

        var sensorDto = new UpdateSensorDto()
        {
            Name = "a", TrackerId = TrackerRepositoryMock.ExistentTrackerId, Description = "description", Formula = "const1",
            ValidatorId = SensorRepositoryMock.ReferencedSensorId, ValidationType = ValidationTypeEnum.LogicalAnd,
            SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, UnitId = UnitRepositoryMock.ExistentUnitId,
            DataType = SensorDataTypeEnum.Number, FuelUse = 1, UseLastReceived = false
        };

        var result = controller.UpdateSensor(1, sensorDto);

        result.Should().BeOfType<OkResult>();
        var okResult = result as OkResult;
        okResult!.StatusCode.Should().Be(200);

        SensorRepositoryMock.LastUpdateArgument.Should().NotBeNull();
        if (SensorRepositoryMock.LastUpdateArgument is null) return;
        SensorRepositoryMock.LastUpdateArgument.Name.Should().Be("a");
        SensorRepositoryMock.LastUpdateArgument.TrackerId.Should().Be(TrackerRepositoryMock.ExistentTrackerId);
        SensorRepositoryMock.LastUpdateArgument.Description.Should().Be("description");
        SensorRepositoryMock.LastUpdateArgument.Formula.Should().Be("const1");
        SensorRepositoryMock.LastUpdateArgument.ValidatorId.Should().Be(SensorRepositoryMock.ReferencedSensorId);
        SensorRepositoryMock.LastUpdateArgument.ValidationType.Should().Be(ValidationTypeEnum.LogicalAnd);
        SensorRepositoryMock.LastUpdateArgument.SensorTypeId.Should().Be(SensorTypeRepositoryMock.ExistentSensorTypeId);
        SensorRepositoryMock.LastUpdateArgument.UnitId.Should().Be(UnitRepositoryMock.MeterUnitId);
        SensorRepositoryMock.LastUpdateArgument.DataType.Should().Be(SensorDataTypeEnum.Number);
        SensorRepositoryMock.LastUpdateArgument.FuelUse.Should().Be(1);
        SensorRepositoryMock.LastUpdateArgument.UseLastReceived.Should().Be(false);
    }

    [Fact]
    public void UpdateSensor_SensorTypeWithDataTypeConstraint_SetsDataType()
    {
        var controller = GetController();

        var sensorDto = new UpdateSensorDto()
        {
            Name = "a", TrackerId = TrackerRepositoryMock.ExistentTrackerId, Description = "", ValidatorId = null,
            SensorTypeId = SensorTypeRepositoryMock.SensorTypeWithBooleanDataTypeId, UnitId = UnitRepositoryMock.ExistentUnitId,
            Formula = "const1", DataType = SensorDataTypeEnum.Number, FuelUse = 1, ValidationType = null,
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
            Name = "a", TrackerId = TrackerRepositoryMock.ExistentTrackerId, Description = "", ValidatorId = null,
            SensorTypeId = SensorTypeRepositoryMock.SensorTypeWithSecondUnitId, UnitId = UnitRepositoryMock.MeterUnitId, Formula = "const1",
            DataType = SensorDataTypeEnum.Number, FuelUse = 1, ValidationType = null, UseLastReceived = false
        };

        var result = controller.UpdateSensor(1, sensorDto);

        result.Should().BeOfType<OkResult>();
        var okResult = result as OkResult;
        okResult!.StatusCode.Should().Be(200);

        Assert.Equal(2, SensorRepositoryMock.LastUpdateArgument!.UnitId);
    }

    [Fact]
    public void Delete_ExistingSensor_Ok()
    {
        var controller = GetController();

        var result = controller.DeleteSensor(SensorRepositoryMock.ExistentSensorId);

        result.Should().BeOfType<OkResult>();
        var okResult = result as OkResult;
        okResult!.StatusCode.Should().Be(200);
    }
    
    [Fact]
    public void Delete_NonExistentSensor_NotFound()
    {
        var controller = GetController();

        var result = controller.DeleteSensor(SensorRepositoryMock.NonExistentSensorId);

        result.Should().BeOfType<NotFoundObjectResult>();
        var badResult = result as NotFoundObjectResult;
        badResult!.StatusCode.Should().Be(404);
        var content = badResult.Value.As<ServiceErrorResult>();
        content.Messages.Length.Should().Be(1);
        content.Messages[0].Should().Match("*не найден*");
    }

    [Fact]
    public void Delete_Conflict_ThrowConflict()
    {
        var controller = GetController();

        var result = controller.DeleteSensor(SensorRepositoryMock.ReferencedSensorId);

        result.Should().BeOfType<ConflictObjectResult>();
        var badResult = result as ConflictObjectResult;
        badResult!.StatusCode.Should().Be(409);
        var content = badResult.Value.As<ServiceErrorResult>();
        content.Messages.Length.Should().Be(1);
        content.Messages[0].Should().Match("*ссылается*");
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
            new SensorValidator(TrackerRepositoryMock.GetStub(), UnitRepositoryMock.GetStub(),
                SensorTypeRepositoryMock.GetStub()), new SensorsRequestValidator(), TrackerRepositoryMock.GetStub(),
            LoggerMock.GetStub<SensorController>(), TrackerTagRepositoryMock.GetStub());
    }
}
