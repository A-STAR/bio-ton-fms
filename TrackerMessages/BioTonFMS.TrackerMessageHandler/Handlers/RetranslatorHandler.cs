using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.TrackerMessageHandler.Retranslation;

namespace BioTonFMS.TrackerMessageHandler.Handlers;

public class RetranslatorHandler : IBusMessageHandler
{
    private readonly IRetranslator _retranslator;

    public RetranslatorHandler(IRetranslator retranslator)
    {
        _retranslator = retranslator;
    }

    public async Task HandleAsync(byte[] binaryMessage)
    {
        await _retranslator.Retranslate(binaryMessage);
    }
}