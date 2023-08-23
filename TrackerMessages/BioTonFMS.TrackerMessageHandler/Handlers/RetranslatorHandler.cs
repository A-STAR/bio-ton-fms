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

    public async Task HandleAsync(byte[] binaryMessage, MessageDeliverEventArgs messageDeliverEventArgs)
    {
        try
        {
            await _retranslator.Retranslate(binaryMessage);
            _retranslationBus.Ack(messageDeliverEventArgs.DeliveryTag, multiple: false);
        }
        catch
        {
            _retranslationBus.Nack(messageDeliverEventArgs.DeliveryTag, multiple: false, requeue: false);
            throw;
        }
    }
}