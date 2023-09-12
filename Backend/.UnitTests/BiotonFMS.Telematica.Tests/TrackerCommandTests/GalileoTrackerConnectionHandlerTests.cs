using System.Net;
using System.IO.Pipelines;
using Microsoft.AspNetCore.Connections;

using Moq;

using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.TrackerTcpServer;
using Microsoft.Extensions.Logging;
using BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;


namespace BiotonFMS.Telematica.Tests.TrackerCommandTests;

public class GalileoTrackerConnectionHandlerTests
{
    [Fact]
    public async Task CheckAndSendCommand_ShouldSendCommand_IfCommandPresent()
    {
        var logStub = new Mock<ILogger<GalileoTrackerConnectionHandler>>();
        var commandMessages = new TcpSendCommandMessages();

        var protocolMessageHandlerMock = new Mock<IProtocolMessageHandler>();
        var handlerProvider = (Func<TrackerTypeEnum, IProtocolMessageHandler>)(key => key switch
        {
            TrackerTypeEnum.GalileoSkyV50 => protocolMessageHandlerMock.Object,
            _ => throw new NotImplementedException()
        });

        var trackerConnectionHandler = new GalileoTrackerConnectionHandler(logStub.Object, handlerProvider, commandMessages);

        var pipeWriter = new Mock<PipeWriter>();
        pipeWriter.Setup(p => p.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()));
        var transportMock = new Mock<IDuplexPipe>();
        transportMock.Setup(t => t.Output).Returns(pipeWriter.Object);
        var connectionContextMock = new Mock<ConnectionContext>();
        connectionContextMock.Setup(c => c.Transport).Returns(transportMock.Object);

        var ipAddress = IPAddress.Parse("10.10.10.10");
        var port = 7070;
        var testMsg = new TrackerCommandMessage();
        testMsg.IpAddress = ipAddress.ToString();
        testMsg.Port = port;
        testMsg.EncodedCommand = new byte[] { 1, 2, 3 };

        commandMessages.AddSendCommandMessage(testMsg);

        await trackerConnectionHandler.CheckAndSendCommand(connectionContextMock.Object, ipAddress, port);

        pipeWriter.Verify(p => p.WriteAsync(testMsg.EncodedCommand, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task CheckAndSendCommand_ShouldNotSendCommand_IfCommandNotPresent()
    {
        var logStub = new Mock<ILogger<GalileoTrackerConnectionHandler>>();
        var commandMessages = new TcpSendCommandMessages();

        var protocolMessageHandlerMock = new Mock<IProtocolMessageHandler>();
        var handlerProvider = (Func<TrackerTypeEnum, IProtocolMessageHandler>)(key => key switch
        {
            TrackerTypeEnum.GalileoSkyV50 => protocolMessageHandlerMock.Object,
            _ => throw new NotImplementedException()
        });

        var trackerConnectionHandler = new GalileoTrackerConnectionHandler(logStub.Object, handlerProvider, commandMessages);

        var pipeWriter = new Mock<PipeWriter>();
        pipeWriter.Setup(p => p.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()));
        var transportMock = new Mock<IDuplexPipe>();
        transportMock.Setup(t => t.Output).Returns(pipeWriter.Object);
        var connectionContextMock = new Mock<ConnectionContext>();
        connectionContextMock.Setup(c => c.Transport).Returns(transportMock.Object);

        var ipAddress = IPAddress.Parse("10.10.10.10");
        var port = 7070;

        await trackerConnectionHandler.CheckAndSendCommand(connectionContextMock.Object, ipAddress, port);

        pipeWriter.Verify(p => p.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Never());
    }
}
