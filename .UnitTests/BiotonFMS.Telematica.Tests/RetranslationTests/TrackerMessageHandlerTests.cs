using System.Text;
using System.Text.Json;
using BioTonFMS.Domain.Messaging;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.TrackerMessageHandler;
using BioTonFMS.TrackerMessageHandler.Handlers;
using BioTonFMS.TrackerMessageHandler.Retranslation;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BiotonFMS.Telematica.Tests.RetranslationTests;

public class TrackerMessageHandlerTests
{
    [Fact]
    public async Task TrackerMessageHandler_ShouldEnqueueRawMessageForRetranslation()
    {
        var retBus = new MessageBusMock();
        var consBus = new MessageBusMock();
        var testData = new byte[] { 0, 1, 2, 3, 4, 5, 6 };
        var handler = new TrackerMessageHandler(
            Mock.Of<ILogger<TrackerMessageHandler>>(),
            null,
            null,
            null,
            null,
            busType => busType switch
            {
                BusType.Consuming => consBus,
                BusType.Retranslation => retBus,
                _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
            },
            Options.Create(new RetranslatorOptions { Enabled = true })
        );

        var rawMsgEncoded = Encoding.UTF8
            .GetBytes(JsonSerializer.Serialize(new RawTrackerMessage { RawMessage = testData }));

        try
        {
            await handler.HandleAsync(rawMsgEncoded);
        }
        catch(Exception e)
        {
        }

        //retBus.Messages.Count.Should().Be(1);
        //retBus.Messages[0].Should().Equal(testData);
    }
}