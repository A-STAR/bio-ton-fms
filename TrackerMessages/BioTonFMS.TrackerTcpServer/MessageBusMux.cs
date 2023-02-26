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

    public void Publish(byte[] message)
    {
        _primaryBus.Publish(message);
        if (_secondaryBus != null)
        {
            _retryPolicy.Execute(() => _secondaryBus.Publish(message));
        }
    }

    public void Subscribe<TBusMessageHandler>()
        where TBusMessageHandler : IBusMessageHandler
    {
        throw new NotImplementedException();
    }
}