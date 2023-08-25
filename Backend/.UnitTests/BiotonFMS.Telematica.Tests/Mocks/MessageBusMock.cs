using BioTonFMS.Infrastructure.MessageBus;

namespace BiotonFMS.Telematica.Tests.Mocks;

public class MessageBusMock : IMessageBus
{
    public readonly List<byte[]> Messages = new();

    public void Ack(ulong deliveryTag, bool multiple)
    {
        throw new NotImplementedException();
    }

    public void Nack(ulong deliveryTag, bool multiple, bool requeue)
    {
        throw new NotImplementedException();
    }

    public void Publish(byte[] message)
    {
        Messages.Add(message);
    }

    public void Subscribe<TBusMessageHandler>() where TBusMessageHandler : IBusMessageHandler
    {
        throw new NotImplementedException();
    }
}