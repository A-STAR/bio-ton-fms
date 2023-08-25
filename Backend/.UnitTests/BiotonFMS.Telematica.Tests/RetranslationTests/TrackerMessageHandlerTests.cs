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

namespace BiotonFMS.Telematica.Tests.RetranslationTests;

public class TrackerMessageHandlerTests
{
    /*[Fact]
    public async Task TrackerMessageHandler_ShouldEnqueueRawMessageForRetranslation()
    {
        var retBus = new MessageBusMock();
        var consBus = new MessageBusMock();
        var commandReceiveBus = new MessageBusMock();
        var testData = new byte[] { 0, 1, 2, 3, 4, 5, 6 };
        var parserProviderMock = (Func<TrackerTypeEnum, ITrackerMessageParser>)(_ => new GalileoskyMessageParser(ProtocolTagRepositoryMock.GetStub(), Mock.Of<ILogger<GalileoskyMessageParser>>()));
        var busResolver = (Func<MessgingBusType, IMessageBus>)(busType => busType switch
            {
                MessgingBusType.Consuming => consBus,
                MessgingBusType.Retranslation => retBus,
                MessgingBusType.TrackerCommandsReceive => commandReceiveBus,
                _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
            });
        var codecResolver = (Func<TrackerTypeEnum, ICommandCodec>)(_ => new GalileoskyCommandCodec());
        var handler = new TrackerMessageHandler(
            Mock.Of<ILogger<TrackerMessageHandler>>(), 
            parserProviderMock,
            codecResolver,
            Mock.Of<ITrackerMessageRepository>(),
            TrackerTagRepositoryMock.GetStub(),
            TrackerRepositoryMock.GetStub(),
            busResolver,
            Options.Create(new RetranslatorOptions { Enabled = true })
        );

        var rawMsgEncoded = Encoding.UTF8
            .GetBytes(JsonSerializer.Serialize(new RawTrackerMessage { RawMessage = testData }));

        try
        {
            await handler.HandleAsync(rawMsgEncoded);
        }
        catch(Exception)
        {
        }

        retBus.Messages.Count.Should().Be(1);
        retBus.Messages[0].Should().Equal(rawMsgEncoded);
    }*/
}