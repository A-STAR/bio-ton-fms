using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos;
using BioTonFMS.Telematica.Mapping;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.Telematica.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

public class VehicleControllerTests
{
    private const int NonexistentEntityId = -1;
    private const int ExistingEntityId = 1;
    
    #region GetVehicle

    [Fact]
    public void GetVehicle_VehicleExists_ShouldReturn200Status()
    {
        var controller = GetController();

        var result = (OkObjectResult)controller.GetVehicle(ExistingEntityId);

        result.StatusCode.Should().Be(200);

        var content = result.Value.As<VehicleDto>();
        content.Should().NotBeNull();
    }

    [Fact]
    public void GetVehicle_VehicleNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var result = (NotFoundObjectResult)controller.GetVehicle(NonexistentEntityId);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Машина с id = {NonexistentEntityId} не найдена");
    }

    #endregion

    #region AddVehicle

    [Fact]
    public void AddVehicle_WithValidData_ShouldReturn200Status()
    {
        var controller = GetController();

        var request = new CreateVehicleDto
        {
                Name = "Синий Трактор",
                Make = "Трактор",
                Model = "Синий",
                FuelTypeId = ExistingEntityId,
                ManufacturingYear = 1991,
                Type = VehicleTypeEnum.Transport,
                SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (OkObjectResult)controller.AddVehicle(request);

        result.StatusCode.Should().Be(200);

        var content = result.Value.As<VehicleDto>();
        content.Should().NotBeNull();
        Assert.Equal(content.Name, request.Name);
        Assert.Equal(content.Make, request.Make);
        Assert.Equal(content.Model, request.Model);
        Assert.Equal(content.ManufacturingYear, request.ManufacturingYear);
        Assert.Equal(content.Type.Value, request.Type.GetDescription());
    }

    [Fact]
    public void AddVehicle_FuelTypeNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var request = new CreateVehicleDto
        {
                Name = "Синий Трактор",
                Make = "Трактор",
                Model = "Синий",
                FuelTypeId = NonexistentEntityId,
                ManufacturingYear = 1991,
                Type = VehicleTypeEnum.Transport,
                SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (NotFoundObjectResult)controller.AddVehicle(request);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Тип топлива с id = {NonexistentEntityId} не найден");
    }

    [Fact]
    public void AddVehicle_VehicleGroupNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var request = new CreateVehicleDto
        {
                Name = "Синий Трактор",
                Make = "Трактор",
                Model = "Синий",
                FuelTypeId = ExistingEntityId,
                VehicleGroupId = NonexistentEntityId,
                ManufacturingYear = 1991,
                Type = VehicleTypeEnum.Transport,
                SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (NotFoundObjectResult)controller.AddVehicle(request);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Группа машин с id = {NonexistentEntityId} не найдена");
    }

    [Fact]
    public void AddVehicle_TrackerNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var request = new CreateVehicleDto
        {
                Name = "Синий Трактор",
                Make = "Трактор",
                Model = "Синий",
                FuelTypeId = ExistingEntityId,
                TrackerId = NonexistentEntityId,
                ManufacturingYear = 1991,
                Type = VehicleTypeEnum.Transport,
                SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (NotFoundObjectResult)controller.AddVehicle(request);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Трекер с id = {NonexistentEntityId} не найден");
    }

    #endregion
    
    #region UpdateVehicle
    
    [Fact]
    public void UpdateVehicle_VehicleExists_ShouldReturn200Status()
    {
        var controller = GetController();

        var request = new UpdateVehicleDto
        {
            Name = "Синий Трактор",
            Make = "Трактор",
            Model = "Синий",
            FuelTypeId = ExistingEntityId,
            ManufacturingYear = 1991,
            Type = VehicleTypeEnum.Transport,
            SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (OkResult)controller.UpdateVehicle(ExistingEntityId, request);

        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public void UpdateVehicle_VehicleNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var request = new UpdateVehicleDto
        {
            Name = "Синий Трактор",
            Make = "Трактор",
            Model = "Синий",
            FuelTypeId = ExistingEntityId,
            ManufacturingYear = 1991,
            Type = VehicleTypeEnum.Transport,
            SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (NotFoundObjectResult)controller.UpdateVehicle(NonexistentEntityId, request);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Машина с id = {NonexistentEntityId} не найдена");
    }

    [Fact]
    public void UpdateVehicle_FuelTypeNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var request = new UpdateVehicleDto
        {
            Name = "Синий Трактор",
            Make = "Трактор",
            Model = "Синий",
            FuelTypeId = NonexistentEntityId,
            ManufacturingYear = 1991,
            Type = VehicleTypeEnum.Transport,
            SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (NotFoundObjectResult)controller.UpdateVehicle(ExistingEntityId, request);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Тип топлива с id = {NonexistentEntityId} не найден");
    }

    [Fact]
    public void UpdateVehicle_TrackerNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var request = new UpdateVehicleDto
        {
            Name = "Синий Трактор",
            Make = "Трактор",
            Model = "Синий",
            FuelTypeId = ExistingEntityId,
            TrackerId = NonexistentEntityId,
            ManufacturingYear = 1991,
            Type = VehicleTypeEnum.Transport,
            SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (NotFoundObjectResult)controller.UpdateVehicle(ExistingEntityId, request);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Трекер с id = {NonexistentEntityId} не найден");
    }

    [Fact]
    public void UpdateVehicle_VehicleGroupNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var request = new UpdateVehicleDto
        {
            Name = "Синий Трактор",
            Make = "Трактор",
            Model = "Синий",
            FuelTypeId = ExistingEntityId,
            VehicleGroupId = NonexistentEntityId,
            ManufacturingYear = 1991,
            Type = VehicleTypeEnum.Transport,
            SubType = VehicleSubTypeEnum.Tractor
        };

        var result = (NotFoundObjectResult)controller.UpdateVehicle(ExistingEntityId, request);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Группа машин с id = {NonexistentEntityId} не найдена");
    }

    #endregion

    #region DeleteVehicle

    [Fact]
    public void DeleteVehicle_VehicleExists_ShouldReturn200Status()
    {
        var controller = GetController();

        var result = (OkResult)controller.DeleteVehicle(ExistingEntityId);

        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public void DeleteVehicle_VehicleNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var result = (NotFoundObjectResult)controller.DeleteVehicle(NonexistentEntityId);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Машина с id = {NonexistentEntityId} не найдена");
    }

    #endregion
    
    private static VehicleController GetController()
    {
        var loggerDummy = Mock.Of<ILogger<VehicleController>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new VehicleGroupMappingProfile());
            cfg.AddProfile(new VehicleMappingProfile());
            cfg.AddProfile(new TrackerMappingProfile());
            cfg.AddProfile(new FuelTypeMappingProfile());
        });
        IMapper mapper = new Mapper(mapperConfig);

        return new VehicleController(
            mapper,
            loggerDummy,
            VehicleMock.GetStub(),
            TrackerMock.GetStub(),
            FuelTypeMock.GetStub(),
            VehicleGroupMock.GetStub(),
            new CreateVehicleDtoValidator(),
            new UpdateVehicleDtoValidator(),
            new VehiclesRequestValidator()
        );
    }
}