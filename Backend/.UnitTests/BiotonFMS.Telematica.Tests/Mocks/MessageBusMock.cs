using BioTonFMS.Infrastructure.MessageBus;

namespace BiotonFMS.Telematica.Tests.Mocks;

public class MessageBusMock : IMessageBus
{
    public readonly List<byte[]> Messages = new();
    private IPublisherConfirmsHandler? _confirmHandler = null;

    public void Ack(ulong deliveryTag, bool multiple)
    {
        throw new NotImplementedException();
    }

    public void Nack(ulong deliveryTag, bool multiple, bool requeue)
    {
        throw new NotImplementedException();
    }

    public ulong Publish(byte[] message)
    {
        Messages.Add(message);
        return 0;
    }

    public void Subscribe<TBusMessageHandler>() where TBusMessageHandler : IBusMessageHandler
    {
        throw new NotImplementedException();
    }

    public void SetPublisherConfirmsHandler(IPublisherConfirmsHandler confirmHandler)
    {
        _confirmHandler = confirmHandler;
    }
}