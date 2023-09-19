using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.TrackerMessageHandler.Retranslation;

namespace BioTonFMS.TrackerMessageHandler.Handlers;

public class RetranslatorHandler : IBusMessageHandler
{
    private readonly IRetranslator _retranslator;
    private readonly IMessageBus _retranslationBus;

    public RetranslatorHandler(IRetranslator retranslator,
        Func<MessgingBusType, IMessageBus> busResolver)
    {
        _retranslator = retranslator;
        _retranslationBus = busResolver(MessgingBusType.Retranslation);
    }

    public async Task HandleAsync(byte[] binaryMessage, ulong deliveryTag)
    {
        try
        {
            await _retranslator.Retranslate(binaryMessage);
            _retranslationBus.Ack(deliveryTag, multiple: false);
        }
        catch
        {
            _retranslationBus.Nack(deliveryTag, multiple: false, requeue: false);
            throw;
        }
    }
}