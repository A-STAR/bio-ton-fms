using BioTonFMS.Infrastructure.MessageBus;
using Polly;

namespace BioTonFMS.TrackerTcpServer;

internal class MessageBusMux : IMessageBus
{
    private readonly IMessageBus _primaryBus;
    private readonly IMessageBus? _secondaryBus;
    private readonly Policy _retryPolicy;

    public MessageBusMux(IMessageBus primaryBus, IMessageBus? secondaryBus, Policy retryPolicy)
    {
        _primaryBus = primaryBus;
        _secondaryBus = secondaryBus;
        _retryPolicy = retryPolicy;
    }

    public void Ack(ulong deliveryTag, bool multiple)
    {
        throw new NotImplementedException();
    }

    public void Nack(ulong deliveryTag, bool multiple, bool requeue)
    {
        throw new NotImplementedException();
    }

    public void SetPublisherConfirmsHandler(IPublisherConfirmsHandler confirmHandler)
    {
        _primaryBus.SetPublisherConfirmsHandler(confirmHandler);
        _secondaryBus?.SetPublisherConfirmsHandler(confirmHandler);
    }

    public ulong Publish(byte[] message)
    {
        var publishNumber = _primaryBus.Publish(message);
        if (_secondaryBus is not null)
        {
            _retryPolicy.Execute(() => _secondaryBus.Publish(message));
        }
        return publishNumber;
    }

    public void Subscribe<TBusMessageHandler>()
        where TBusMessageHandler : IBusMessageHandler
    {
        throw new NotImplementedException();
    }
}