using Moq;

using BioTonFMS.Domain;
using BiotonFMS.Telematica.Tests.Mocks;
using Microsoft.Extensions.Logging;
using BioTonFMS.TrackerMessageHandler.Handlers;
using BioTonFMS.TrackerProtocolSpecific.TrackerMessages;
using BioTonFMS.TrackerProtocolSpecific.CommandCodecs;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.TrackerMessageHandler.Retranslation;
using Microsoft.Extensions.Options;
using BioTonFMS.Domain.Messaging;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using BioTonFMS.Common.Testable;

namespace BiotonFMS.Telematica.Tests.TrackerCommandTests;

public class TrackerMessageHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldAckConsumerBus_IfCommandReplyProcessed()
    {
        var logStub = new Mock<ILogger<TrackerMessageHandler>>();
        var parserProviderMock = (Func<TrackerTypeEnum, ITrackerMessageParser>)(_ => new GalileoskyMessageParser(ProtocolTagRepositoryMock.GetStub(), Mock.Of<ILogger<GalileoskyMessageParser>>()));
        var codecProvider = (Func<TrackerTypeEnum, ICommandCodec>)(_ => new GalileoskyCommandCodec());
        var trackerMessageRepository = new Mock<ITrackerMessageRepository>();
        var trackerTagRepository = new Mock<ITrackerTagRepository>();
        var trackerRepository = new Mock<ITrackerRepository>();
        var retBus = new MessageBusMock();
        var consBus = new MessageBusMock();
        var commandReceiveBus = new MessageBusMock();
        var busResolver = (Func<MessgingBusType, IMessageBus>)(busType => busType switch
        {
            MessgingBusType.Consuming => consBus,
            MessgingBusType.Retranslation => retBus,
            MessgingBusType.TrackerCommandsReceive => commandReceiveBus,
            _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
        });

        var trackerMessageHandler = new TrackerMessageHandler(logStub.Object, parserProviderMock, codecProvider, 
            trackerMessageRepository.Object, trackerTagRepository.Object, trackerRepository.Object,
            busResolver, Options.Create(new RetranslatorOptions { Enabled = false }));

        var commandReply = new byte[] { 0x1, 0x2E, 0x0, 0x3, 0x38, 0x36, 0x38, 0x33, 0x34, 0x35, 0x30, 0x33, 0x32, 0x30, 0x37, 0x33, 0x35, 0x34, 0x36, 0x4, 0xAC, 0x6, 0xE0, 0x48, 0x0, 0x0,
                0x0, 0xE1, 0x14, 0x49, 0x4D, 0x45, 0x49, 0x20, 0x38, 0x36, 0x38, 0x33, 0x34, 0x35, 0x30, 0x33, 0x32, 0x30, 0x37, 0x33, 0x35, 0x34, 0x36, 0x59, 0x31 };
        var raw = new RawTrackerMessage
        {
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            IpAddress = "10.10.10.10",
            Port = 0,
            PackageUID = new Guid(),
            RawMessage = commandReply
        };
        ulong testDeliveryTag = 101;
        await trackerMessageHandler.HandleAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(raw)), testDeliveryTag);
        
        consBus.ackList.Contains((testDeliveryTag, false)).Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishCommandResponseMessage_IfCommandReplyProcessed()
    {
        var logStub = new Mock<ILogger<TrackerMessageHandler>>();
        var parserProviderMock = (Func<TrackerTypeEnum, ITrackerMessageParser>)(_ => new GalileoskyMessageParser(ProtocolTagRepositoryMock.GetStub(), Mock.Of<ILogger<GalileoskyMessageParser>>()));
        var codecProvider = (Func<TrackerTypeEnum, ICommandCodec>)(_ => new GalileoskyCommandCodec());
        var trackerMessageRepository = new Mock<ITrackerMessageRepository>();
        var trackerTagRepository = new Mock<ITrackerTagRepository>();
        var trackerRepository = new Mock<ITrackerRepository>();
        var retBus = new MessageBusMock();
        var consBus = new MessageBusMock();
        var commandReceiveBus = new MessageBusMock();
        var busResolver = (Func<MessgingBusType, IMessageBus>)(busType => busType switch
        {
            MessgingBusType.Consuming => consBus,
            MessgingBusType.Retranslation => retBus,
            MessgingBusType.TrackerCommandsReceive => commandReceiveBus,
            _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
        });

        var trackerMessageHandler = new TrackerMessageHandler(logStub.Object, parserProviderMock, codecProvider,
            trackerMessageRepository.Object, trackerTagRepository.Object, trackerRepository.Object,
            busResolver, Options.Create(new RetranslatorOptions { Enabled = false }));

        var commandReply = new byte[] { 0x1, 0x2E, 0x0, 0x3, 0x38, 0x36, 0x38, 0x33, 0x34, 0x35, 0x30, 0x33, 0x32, 0x30, 0x37, 0x33, 0x35, 0x34, 0x36, 0x4, 0xAC, 0x6, 0xE0, 0x48, 0x0, 0x0,
                0x0, 0xE1, 0x14, 0x49, 0x4D, 0x45, 0x49, 0x20, 0x38, 0x36, 0x38, 0x33, 0x34, 0x35, 0x30, 0x33, 0x32, 0x30, 0x37, 0x33, 0x35, 0x34, 0x36, 0x59, 0x31 };
        var raw = new RawTrackerMessage
        {
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            IpAddress = "10.10.10.10",
            Port = 0,
            PackageUID = new Guid(),
            RawMessage = commandReply
        };
        var expectedNow = DateTime.UtcNow;
        SystemTime.Set(expectedNow);
        TrackerCommandResponseMessage expectedCommendResponseMessage = new()
        {
            Imei = "868345032073546",
            ExternalId = 1708,
            CommandId = 72,
            ResponseText = "IMEI 868345032073546",
            ResponseBinary = null,
            ResponseDateTime = expectedNow
        };
        ulong testDeliveryTag = 102;
        await trackerMessageHandler.HandleAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(raw)), testDeliveryTag);
        var expectedMessage = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedCommendResponseMessage));
        var expected = string.Join(' ', expectedMessage.ToArray().Select(x => x.ToString("X")));
        var actual = string.Join(' ', commandReceiveBus.Messages[0].Select(x => x.ToString("X")));

        expected.Should().BeEquivalentTo(actual);
    }


}
