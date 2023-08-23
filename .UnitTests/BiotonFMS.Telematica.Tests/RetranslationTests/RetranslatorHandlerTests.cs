using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.Infrastructure.RabbitMQ;
using BioTonFMS.TrackerMessageHandler.Handlers;
using BioTonFMS.TrackerMessageHandler.Retranslation;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BiotonFMS.Telematica.Tests.RetranslationTests;

public class RetranslatorHandlerTests
{
    [Fact]
    public async Task RetranslatorHandler_ShouldUseRetranslateMethod()
    {
        var mock = new RetranslatorMock();
        Func<MessgingBusType, IMessageBus> busResolver = CreateOrGetBus;

        var handler = new RetranslatorHandler(mock, busResolver);

        await handler.HandleAsync(new byte[] {0,1,2}, new MessageDeliverEventArgs());

        mock.Toggled.Should().BeTrue();
    }

    public static IMessageBus CreateOrGetBus(MessgingBusType busType)
    {
        return Mock.Of<IMessageBus>();
    }
}

internal class RetranslatorMock : IRetranslator
{
    public bool Toggled = false;
    public Task Retranslate(byte[] rawMessage)
    {
        Toggled = true;
        return Task.CompletedTask;
    }
}