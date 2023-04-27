using System.Collections;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos.Parameters;
using BiotonFMS.Telematica.Tests.Mocks;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

public class TrackerDataControllerTests
{
    private const int NonexistentEntityId = -1;
    private const int ExistingEntityId = 1;

    [Fact]
    public void GetParameters_TrackerExists_ShouldReturnParameters()
    {
        var controller = GetController();
        var resp = (ObjectResult)controller.GetParameters(ExistingEntityId);

        resp.StatusCode.Should().Be(200);

        var content = resp.Value.As<IList<TrackerParameter>>();
        content.Should().NotBeNull();
        content.Count.Should().Be(3);
    }
    
    [Fact]
    public void GetParameters_TrackerNotExists_ShouldReturn404Status()
    {
        var controller = GetController();
        var resp = (NotFoundObjectResult)controller.GetParameters(NonexistentEntityId);

        resp.StatusCode.Should().Be(404);

        var content = resp.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Трекер с id = {NonexistentEntityId} не найден");
    }

    [Fact]
    public void GetStandardParameters_TrackerExists_ShouldReturnStandardParameters()
    {
        var controller = GetController();
        var resp = (ObjectResult)controller.GetStandardParameters(ExistingEntityId);

        resp.StatusCode.Should().Be(200);

        var content = resp.Value.As<TrackerStandardParameter[]>();
        content.Should().NotBeNull();
        content.Length.Should().Be(5);
    }

    [Fact]
    public void GetStandardParameters_TrackerNotExists_ShouldReturn404Status()
    {
        var controller = GetController();
        var resp = (NotFoundObjectResult)controller.GetStandardParameters(NonexistentEntityId);

        resp.StatusCode.Should().Be(404);

        var content = resp.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Трекер с id = {NonexistentEntityId} не найден");
    }

    [Fact]
    public void GetParametersHistory_TrackerExists_ShouldReturnParametersHistory()
    {
        var controller = GetController();
        var req = new ParametersHistoryRequest
        {
            TrackerId = ExistingEntityId,
            PageNum = 1,
            PageSize = 10
        };
        var resp = (ObjectResult)controller.GetParametersHistory(req);

        resp.StatusCode.Should().Be(200);

        var content = resp.Value.As<ParametersHistoryResponse>();
        content.Should().NotBeNull();
    }

    [Fact]
    public void GetParametersHistory_TrackerNotExists_ShouldReturn404Status()
    {
        var controller = GetController();
        var resp = (NotFoundObjectResult)controller.GetParametersHistory(new ParametersHistoryRequest
        {
            TrackerId = NonexistentEntityId,
            PageNum = 1,
            PageSize = 10
        });

        resp.StatusCode.Should().Be(404);

        var content = resp.Value.As<ServiceErrorResult>();
        content.Should().NotBeNull();
        Assert.Equal(content.Messages[0], $"Трекер с id = {NonexistentEntityId} не найден");
    }

    private static TrackerDataController GetController()
    {
        var trackersRepo = TrackerRepositoryMock.GetStub();
        
        return new TrackerDataController(GetRepo(), trackersRepo);
    }

    private static ITrackerMessageRepository GetRepo()
    {
        var messages = new TrackerMessage[]
        {
            new()
            {
                Id = 0,
                ExternalTrackerId = 111,
                Imei = "12341",
                ServerDateTime = DateTime.UtcNow,
                TrackerDateTime = DateTime.UtcNow,
                Latitude = 49.432023,
                Longitude = 52.556861,
                SatNumber = 12,
                CoordCorrectness = 0,
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
                Id = 1,
                ExternalTrackerId = 111,
                Imei = "12341",
                ServerDateTime = DateTime.UtcNow + TimeSpan.FromSeconds(2),
                TrackerDateTime = DateTime.UtcNow + TimeSpan.FromSeconds(2),
                Latitude = 49.432023,
                Longitude = 52.556861,
                SatNumber = 12,
                CoordCorrectness = 0,
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
            }
        };

        var kvp = new KeyValueProviderMock<TrackerMessage, int>(messages);
        var qp = new QueryableProviderMock<TrackerMessage>(messages);
        var uow = new MessagesDBContextUnitOfWorkFactoryMock();
        var repo = new TrackerMessageRepository(kvp, qp, uow, TrackerTagRepositoryMock.GetStub());
        return repo;
    }
}