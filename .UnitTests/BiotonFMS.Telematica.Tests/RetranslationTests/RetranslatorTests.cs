using System.Text;
using System.Text.Json;
using BioTonFMS.Domain.Messaging;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.TrackerMessageHandler.Retranslation;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.RetranslationTests;

public class RetranslatorTests
{
    [Fact]
    public async Task Retranslate_ShouldWriteToStream()
    {
        var options = new RetranslatorOptions { TimeoutSeconds = 10 };
        var stream = new MemoryStream();
        var client = new TcpClientMock(stream);
        var testData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        var testData2 = new byte[] { 1, 1, 1, 1 };
        var logger = new LoggerMock<Retranslator>();
        var retranslator = new Retranslator(logger, Options.Create(options), () => client);

        var rawMsgEncoded = Encoding.UTF8
            .GetBytes(JsonSerializer.Serialize(new RawTrackerMessage { RawMessage = testData }));

        stream.Position = 8;
        stream.Write(testData2);
        stream.Position = 0;

        await retranslator.Retranslate(rawMsgEncoded);

        stream.Position = 0;
        var actualData = new byte[testData.Length];
        var readCount = stream.Read(actualData);

        readCount.Should().Be(testData.Length);
        actualData.Should().Equal(testData);
    }
}