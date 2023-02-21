using BioTonFMS.Infrastructure.MessageBus;

namespace BioTonFMS.TrackerTcpServer;

public class MessageBusMux : IMessageBus
{
    private readonly IMessageBus _primary;
    private readonly IMessageBus _secondary;

    public MessageBusMux(IMessageBus secondary, IMessageBus primary)
    {
        _secondary = secondary;
        _primary = primary;
    }

    public void Publish(byte[] message)
    {
        _primary.Publish(message);
        _secondary.Publish(message);
    }

    public void Subscribe<TBusMessageHandler>()
        where TBusMessageHandler : IBusMessageHandler
    {
        throw new NotImplementedException();
    }
}