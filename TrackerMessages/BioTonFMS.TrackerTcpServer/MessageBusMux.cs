using BioTonFMS.Infrastructure.MessageBus;
using Polly;

namespace BioTonFMS.TrackerTcpServer;

internal class MessageBusMux : IMessageBus
{
    private readonly IMessageBus _primary;
    private readonly IMessageBus _secondary;
    private readonly Policy _policy;

    public MessageBusMux(IMessageBus secondary, IMessageBus primary, Policy policy)
    {
        _secondary = secondary;
        _primary = primary;
        _policy = policy;
    }

    public void Publish(byte[] message)
    {
        _primary.Publish(message);
        _policy.Execute(() => _secondary.Publish(message));
    }

    public void Subscribe<TBusMessageHandler>()
        where TBusMessageHandler : IBusMessageHandler
    {
        throw new NotImplementedException();
    }
}