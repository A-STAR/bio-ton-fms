using System.Net;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.TrackerTcpServer;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.TrackerCommandTests;

public class TcpSendCommandMessagesTests
{
    [Fact]
    public void TcpSendCommandMessages_ShouldAddSingleCommand_And_ReturnItForTheSameAddress()
    {
        var commandMessages = new TcpSendCommandMessages();

        var ipAddress = IPAddress.Parse("10.10.10.10");
        var port = 7070;
        var testMsg = new TrackerCommandMessage();
        testMsg.IpAddress = ipAddress.ToString();
        testMsg.Port = port;
        testMsg.EncodedCommand = new byte[] { 1, 2, 3 };

        commandMessages.AddSendCommandMessage(testMsg);

        var commandMessage = commandMessages.GetCommandForTracker(ipAddress, port);
        commandMessage.Should().NotBeNull();
        commandMessage.Should().BeEquivalentTo(testMsg);
    }

    [Fact]
    public void TcpSendCommandMessages_ShouldAddSingleCommand_And_ReturnNullForAnotherAddress()
    {
        var commandMessages = new TcpSendCommandMessages();

        var ipAddress = IPAddress.Parse("10.10.10.10");
        var port = 7070;
        var testMsg = new TrackerCommandMessage();
        testMsg.IpAddress = ipAddress.ToString();
        testMsg.Port = port;
        testMsg.EncodedCommand = new byte[] { 1, 2, 3 };

        commandMessages.AddSendCommandMessage(testMsg);

        var anotherIpAddress = IPAddress.Parse("20.20.20.20");
        var commandMessage = commandMessages.GetCommandForTracker(anotherIpAddress, port);
        commandMessage.Should().BeNull();
    }

    [Fact]
    public void TcpSendCommandMessages_ShouldReturnCommandsInOrderOfAdding()
    {
        var commandMessages = new TcpSendCommandMessages();

        var ipAddress = IPAddress.Parse("10.10.10.10");
        var port = 7070;
        var testMsg1 = new TrackerCommandMessage();
        testMsg1.IpAddress = ipAddress.ToString();
        testMsg1.Port = port;
        testMsg1.EncodedCommand = new byte[] { 1, 2, 3 };
        commandMessages.AddSendCommandMessage(testMsg1);

        var testMsg2 = new TrackerCommandMessage();
        testMsg2.IpAddress = ipAddress.ToString();
        testMsg2.Port = port;
        testMsg2.EncodedCommand = new byte[] { 3, 2, 1 };
        commandMessages.AddSendCommandMessage(testMsg2);

        var commandMessage = commandMessages.GetCommandForTracker(ipAddress, port);
        commandMessage.Should().NotBeNull();
        commandMessage.Should().BeEquivalentTo(testMsg1);

        commandMessage = commandMessages.GetCommandForTracker(ipAddress, port);
        commandMessage.Should().NotBeNull();
        commandMessage.Should().BeEquivalentTo(testMsg2);
    }

    [Fact]
    public void TcpSendCommandMessages_ShouldReturnCommandsForCorrespondingIp()
    {
        var commandMessages = new TcpSendCommandMessages();

        var ipAddress = IPAddress.Parse("10.10.10.10");
        var port = 7070;
        var testMsg1 = new TrackerCommandMessage();
        testMsg1.IpAddress = ipAddress.ToString();
        testMsg1.Port = port;
        testMsg1.EncodedCommand = new byte[] { 1, 2, 3 };
        commandMessages.AddSendCommandMessage(testMsg1);

        var anotherIpAddress = IPAddress.Parse("20.20.20.20");
        var testMsg2 = new TrackerCommandMessage();
        testMsg2.IpAddress = anotherIpAddress.ToString();
        testMsg2.Port = port;
        testMsg2.EncodedCommand = new byte[] { 3, 2, 1 };
        commandMessages.AddSendCommandMessage(testMsg2);

        var commandMessage = commandMessages.GetCommandForTracker(ipAddress, port);
        commandMessage.Should().NotBeNull();
        commandMessage.Should().BeEquivalentTo(testMsg1);

        commandMessage = commandMessages.GetCommandForTracker(anotherIpAddress, port);
        commandMessage.Should().NotBeNull();
        commandMessage.Should().BeEquivalentTo(testMsg2);
    }
}
