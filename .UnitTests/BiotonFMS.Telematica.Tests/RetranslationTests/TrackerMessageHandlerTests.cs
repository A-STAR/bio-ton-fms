using System.Text;
using System.Text.Json;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.TrackerMessageHandler;
using BioTonFMS.TrackerMessageHandler.Handlers;
using BioTonFMS.TrackerMessageHandler.MessageParsing;
using BioTonFMS.TrackerMessageHandler.Retranslation;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using BioTonFMS.Domain;
using System;
using BioTonFMS.Infrastructure.MessageBus;

namespace BiotonFMS.Telematica.Tests.RetranslationTests;

public class TrackerMessageHandlerTests
{
    [Fact]
    public async Task TrackerMessageHandler_ShouldEnqueueRawMessageForRetranslation()
    {
        var retBus = new MessageBusMock();
        var consBus = new MessageBusMock();
        var testData = new byte[] { 0, 1, 2, 3, 4, 5, 6 };
        var parserProviderMock = (Func<TrackerTypeEnum, IMessageParser>)(_ => new GalileoskyMessageParser(ProtocolTagRepositoryMock.GetStub(), Mock.Of<ILogger<GalileoskyMessageParser>>()));
        var busResolver = (Func<BusType, IMessageBus>)(busType => busType switch
            {
                BusType.Consuming => consBus,
                BusType.Retranslation => retBus,
                _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
            });

        var handler = new TrackerMessageHandler(
            Mock.Of<ILogger<TrackerMessageHandler>>(), 
            parserProviderMock, Mock.Of<ITrackerMessageRepository>(),
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
    }
}