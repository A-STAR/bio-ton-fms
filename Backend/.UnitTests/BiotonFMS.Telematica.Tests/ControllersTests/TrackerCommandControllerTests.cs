using BioTonFMS.Common.Settings;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.Persistence.Specifications;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Dtos.TrackerCommand;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using BioTonFMS.TrackerProtocolSpecific.Senders;
using FluentAssertions;
using LinqSpecs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BiotonFMS.Telematica.Tests.ControllersTests;

public class TrackerCommandControllerTests
{
    [Fact]
    public async Task SendCommand_TrackerExists_ShouldAddCommandToDb()
    {
        var repository = new TrackerCommandRepositoryMock();
        var controller = GetController(repository);
        const string command = "test cmd";

        await controller.SendCommand(TrackerRepositoryMock.ExistentTrackerId,
            new TrackerCommandDto
            {
                CommandText = command,
                Transport = TrackerCommandTransportEnum.TCP
            });

        repository.Count.Should().Be(1);
        repository[1]?.Should().NotBeNull();
        repository[1]!.CommandText.Should().Be(command);
    }

    [Fact]
    public async Task SendCommand_TrackerNotExists_ShouldReturn404Error()
    {
        var controller = GetController();

        var result = await controller.SendCommand(TrackerRepositoryMock.NonExistentTrackerId,
            new TrackerCommandDto
            {
                CommandText = "",
                Transport = TrackerCommandTransportEnum.TCP
            });

        var response = result.As<NotFoundObjectResult>().Value.As<ServiceErrorResult>().Messages;
        response.Length.Should().Be(1);
        response[0].Should().Be($"Трекер с id = {TrackerRepositoryMock.NonExistentTrackerId} не найден");
    }

    [Fact]
    public async Task SendCommand_WithoutResponse_ShouldReturnError()
    {
        var repository = new TrackerCommandRepositoryMock();
        var controller = GetController(repository);
        const string command = "test cmd";

        var result = await controller.SendCommand(1, new TrackerCommandDto
        {
            CommandText = command,
            Transport = TrackerCommandTransportEnum.TCP
        });

        var response = result.As<ConflictObjectResult>().Value.As<ServiceErrorResult>().Messages;
        response.Length.Should().Be(1);
        response[0].Should().Be("Ответ не получен в течении заданного времени");
    }

    private TrackerCommandController GetController(ITrackerCommandRepository? repo = null)
    {
        var logger = Mock.Of<ILogger<TrackerCommandController>>();
        var trackerRepo = TrackerRepositoryMock.GetStub();
        Func<TrackerTypeEnum, TrackerCommandTransportEnum, ITrackerCommandSender> senderResolver =
            (_, _) => Mock.Of<ITrackerCommandSender>();
        var options = new TrackerOptions { CommandTimeoutSec = 1 };
        var commandRepo = repo ?? new TrackerCommandRepositoryMock();

        var controller = new TrackerCommandController(
            logger, trackerRepo, senderResolver, commandRepo, Options.Create(options)
        );

        return controller;
    }
}

public class TrackerCommandRepositoryMock : ITrackerCommandRepository
{
    private int _idSeq;
    private readonly List<TrackerCommand> _mockList;

    public TrackerCommandRepositoryMock()
    {
        _mockList = new List<TrackerCommand>();
        _idSeq = 0;
    }

    public TrackerCommand? this[int key] => _mockList.FirstOrDefault(x => x.Id == key);

    public TrackerCommand? GetWithoutCaching(int key)
    {
        return _mockList.FirstOrDefault(x => x.Id == key);
    }

    public int Count => _mockList.Count;

    public void Remove(TrackerCommand entity)
    {
        _mockList.Remove(entity);
    }

    public void Add(TrackerCommand entity)
    {
        _idSeq++;
        entity.Id = _idSeq;
        _mockList.Add(entity);
    }

    public void Update(TrackerCommand entity)
    {
        throw new NotImplementedException();
    }

    public IBeginSpecification<TrackerCommand> BeginSpecification(Specification<TrackerCommand> specification)
    {
        throw new NotImplementedException();
    }
}