using BioTonFMS.TrackerMessageHandler.Handlers;
using BioTonFMS.TrackerMessageHandler.Retranslation;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.RetranslationTests;

public class RetranslatorHandlerTests
{
    [Fact]
    public async Task RetranslatorHandler_ShouldUseRetranslateMethod()
    {
        var mock = new RetranslatorMock();
        var handler = new RetranslatorHandler(mock);

        await handler.HandleAsync(new byte[] {0,1,2});

        mock.Toggled.Should().BeTrue();
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