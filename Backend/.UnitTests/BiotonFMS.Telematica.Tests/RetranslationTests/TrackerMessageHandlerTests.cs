using System.Text;
using System.Text.Json;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.MessageBus;
using BiotonFMS.Telematica.Tests.Mocks;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using BioTonFMS.TrackerMessageHandler.Handlers;
using BioTonFMS.TrackerMessageHandler.Retranslation;
using BioTonFMS.TrackerProtocolSpecific.CommandCodecs;
using BioTonFMS.TrackerProtocolSpecific.TrackerMessages;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

namespace BiotonFMS.Telematica.Tests.RetranslationTests;

public class TrackerMessageHandlerTests
{
    [Fact]
    public async Task TrackerMessageHandler_ShouldEnqueueRawMessageForRetranslation()
    {
        var logStub = new Mock<ILogger<TrackerMessageHandler>>();
        var codecProvider = (Func<TrackerTypeEnum, ICommandCodec>)(_ => new GalileoskyCommandCodec());
        var retBus = new MessageBusMock();
        var consBus = new MessageBusMock();
        var commandReceiveBus = new MessageBusMock();
        var trackerMessageRepository = new Mock<ITrackerMessageRepository>();
        var trackerTagRepository = new Mock<ITrackerTagRepository>();
        var trackerRepository = new Mock<ITrackerRepository>();

        var testData = new byte[] { 0x01, 0x17, 0x80, 0x01, 0x82, 0x02, 0x17, 0x03, 0x38, 0x36, 0x32, 0x30, 0x35, 0x37, 0x30, 
            0x34, 0x37, 0x38, 0x33, 0x32, 0x38, 0x32, 0x36, 0x04, 0xCC, 0x07, 0x4A, 0x22 };
        var parserProviderMock = (Func<TrackerTypeEnum, ITrackerMessageParser>)(_ => new GalileoskyMessageParser(ProtocolTagRepositoryMock.GetStub(), Mock.Of<ILogger<GalileoskyMessageParser>>()));
        var busResolver = (Func<MessgingBusType, IMessageBus>)(busType => busType switch
            {
                MessgingBusType.Consuming => consBus,
                MessgingBusType.Retranslation => retBus,
                MessgingBusType.TrackerCommandsReceive => commandReceiveBus,
                _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
            });

        var trackerMessageHandler = new TrackerMessageHandler(logStub.Object, parserProviderMock, codecProvider,
            trackerMessageRepository.Object, trackerTagRepository.Object, trackerRepository.Object,
            busResolver, Options.Create(new RetranslatorOptions { Enabled = true }));

        var rawMsgEncoded = Encoding.UTF8
            .GetBytes(JsonSerializer.Serialize(new RawTrackerMessage { RawMessage = testData }));

        try
        {
            await trackerMessageHandler.HandleAsync(rawMsgEncoded, deliveryTag: 0);
        }
        catch(Exception)
        {
        }

        retBus.Messages.Count.Should().Be(1);
        retBus.Messages[0].Should().Equal(rawMsgEncoded);
    }
}