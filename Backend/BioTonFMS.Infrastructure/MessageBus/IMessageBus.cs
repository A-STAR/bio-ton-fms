namespace BioTonFMS.Infrastructure.MessageBus;

public interface IMessageBus
{
    void SetPublisherConfirmsHandler(IPublisherConfirmsHandler confirmHandler);

    ulong Publish(byte[] message);

    void Subscribe<TBusMessageHandler>()
        where TBusMessageHandler : IBusMessageHandler;

    void Ack(ulong deliveryTag, bool multiple);

    void Nack(ulong deliveryTag, bool multiple, bool requeue);
}
