using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos.Tracker;
using BioTonFMS.Telematica.Mapping;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.Telematica.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using BioTonFMS.Telematica.Dtos.Vehicle;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

public class TrackerControllerTests
{
    private const int NonexistentEntityId = -1;
    private const int ExistingEntityId = 1;
    
    #region GetTracker

    [Fact]
    public void GetTracker_TrackerExists_ShouldReturn200Status()
    {
        var controller = GetController();

        var result = (OkObjectResult)controller.GetTracker(ExistingEntityId);

        result.StatusCode.Should().Be(200);

        var content = result.Value.As<TrackerDto>();
        content.Should().NotBeNull();
    }

    [Fact]
    public void GetTracker_TrackerNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var result = (NotFoundObjectResult)controller.GetTracker(NonexistentEntityId);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Трекер с id = {NonexistentEntityId} не найден");
    }

    #endregion

    #region AddTracker

    [Fact]
    public void AddTracker_WithValidData_ShouldReturn200Status()
    {
        var controller = GetController();

        var request = new CreateTrackerDto
        {
            Name = "трекер GalileoSky",
            Description = "Описание 1",
            Imei = "12341",
            ExternalId = 1111,
            StartDate = DateTime.UtcNow,
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            SimNumber = "905518101010"
        };

        var result = (OkObjectResult)controller.AddTracker(request);

        result.StatusCode.Should().Be(200);

        var content = result.Value.As<TrackerDto>();
        content.Should().NotBeNull();
        Assert.Equal(content.Name, request.Name);
        Assert.Equal(content.Description, request.Description);
        Assert.Equal(content.Imei, request.Imei);
        Assert.Equal(content.ExternalId, request.ExternalId);
        Assert.Equal(content.StartDate, request.StartDate);
        Assert.Equal(content.TrackerType.Value, request.TrackerType.GetDescription());
        Assert.Equal(content.SimNumber, request.SimNumber);
        content.Vehicle.Should().BeNull();
    }

    #endregion
    
    #region UpdateVehicle
    
    [Fact]
    public void UpdateTracker_TrackerExists_ShouldReturn200Status()
    {
        var controller = GetController();

        var request = new UpdateTrackerDto
        {
            Name = "трекер GalileoSky",
            Description = "Описание 1",
            Imei = "12341",
            ExternalId = 111,
            StartDate = DateTime.UtcNow,
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            SimNumber = "905518101010"
        };

        var result = (OkResult)controller.UpdateTracker(ExistingEntityId, request);
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public void UpdateTracker_TrackerNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var request = new UpdateTrackerDto
        {
            Name = "трекер GalileoSky",
            Description = "Описание 1",
            Imei = "12341",
            ExternalId = 111,
            StartDate = DateTime.UtcNow,
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            SimNumber = "905518101010"
        };

        var result = (NotFoundObjectResult)controller.UpdateTracker(NonexistentEntityId, request);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Трекер с id = {NonexistentEntityId} не найден");
    }

    #endregion

    #region DeleteVehicle

    [Fact]
    public void DeleteTracker_TrackerExists_ShouldReturn200Status()
    {
        var entityIdForDelete = 121;
        var controller = GetController();

        var result = (OkResult)controller.DeleteTracker(entityIdForDelete);

        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public void DeleteTracker_TrackerNotExists_ShouldReturn404Status()
    {
        var controller = GetController();

        var result = (NotFoundObjectResult)controller.DeleteTracker(NonexistentEntityId);

        result.StatusCode.Should().Be(404);

        var content = result.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Трекер с id = {NonexistentEntityId} не найден");
    }

    #endregion
    
    private static TrackerController GetController()
    {
        var loggerDummy = Mock.Of<ILogger<TrackerController>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new VehicleGroupMappingProfile());
            cfg.AddProfile(new VehicleMappingProfile());
            cfg.AddProfile(new TrackerMappingProfile());
            cfg.AddProfile(new FuelTypeMappingProfile());
        });
        IMapper mapper = new Mapper(mapperConfig);

        return new TrackerController(
            TrackerRepositoryMock.GetStub(),
            mapper,
            loggerDummy,
            new UpdateTrackerDtoValidator(),
            new CreateTrackerDtoValidator(),
            new TrackersRequestValidator()
        );
    }
}