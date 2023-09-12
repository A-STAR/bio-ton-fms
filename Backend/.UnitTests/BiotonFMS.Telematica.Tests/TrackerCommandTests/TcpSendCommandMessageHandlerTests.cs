using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.TrackerTcpServer;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.TrackerCommandTests;

public class TcpSendCommandMessageHandlerTests
{
    [Fact]
    public void HandleAsync_ShouldAddCommandToCommandMessages()
    {
        var logStub = new Mock<ILogger<TcpSendCommandMessageHandler>>();
        var commandMessagesMock = new Mock<TcpSendCommandMessages>();
        commandMessagesMock.Setup(s => s.AddSendCommandMessage(It.IsAny<TrackerCommandMessage>()));
        var commandSendBus = new MessageBusMock();

        var busResolver = (Func<MessgingBusType, IMessageBus>)(busType => busType switch
        {
            MessgingBusType.TrackerCommandsSend => commandSendBus,
            _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
        });

        var sendCommandHandler = new TcpSendCommandMessageHandler(logStub.Object, commandMessagesMock.Object, busResolver);

        var testMsg = new TrackerCommandMessage();
        var serializedMsg = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(testMsg));
        sendCommandHandler.HandleAsync(serializedMsg, 0);

        commandMessagesMock.Verify(cm => cm.AddSendCommandMessage(It.IsAny<TrackerCommandMessage>()), Times.Once());
    }

    [Fact]
    public void HandleAsync_ShouldAckCommandBusWithDeliveryTag()
    {
        var logStub = new Mock<ILogger<TcpSendCommandMessageHandler>>();
        var commandMessagesMock = new Mock<TcpSendCommandMessages>();
        var commandSendBusMock = new MessageBusMock();

        var busResolver = (Func<MessgingBusType, IMessageBus>)(busType => busType switch
        {
            MessgingBusType.TrackerCommandsSend => commandSendBusMock,
            _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
        });

        var sendCommandHandler = new TcpSendCommandMessageHandler(logStub.Object, commandMessagesMock.Object, busResolver);

        var testMsg = new TrackerCommandMessage();
        var serializedMsg = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(testMsg));
        ulong testDeliveryTag = 101;
        sendCommandHandler.HandleAsync(serializedMsg, testDeliveryTag);

        commandSendBusMock.ackList.Contains((testDeliveryTag, false)).Should().BeTrue();
    }

    [Fact]
    public void HandleAsync_ShouldNackCommandBusWithDeliveryTag_WhenExceptionIsTrown()
    {
        var logStub = new Mock<ILogger<TcpSendCommandMessageHandler>>();
        var commandMessagesMock = new Mock<TcpSendCommandMessages>();
        var commandSendBusMock = new MessageBusMock();

        var busResolver = (Func<MessgingBusType, IMessageBus>)(busType => busType switch
        {
            MessgingBusType.TrackerCommandsSend => commandSendBusMock,
            _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
        });

        var sendCommandHandler = new TcpSendCommandMessageHandler(logStub.Object, commandMessagesMock.Object, busResolver);

        //var testMsg = new TrackerCommandMessage();
        var serializedMsg = Encoding.UTF8.GetBytes(JsonSerializer.Serialize("some wrong thing"));
        ulong testDeviveryTag = 101;
        try
        {
            sendCommandHandler.HandleAsync(serializedMsg, testDeviveryTag);
        }
        catch {
        }
        commandSendBusMock.nackList.Contains((testDeviveryTag, false, false)).Should().BeTrue();
    }
}
