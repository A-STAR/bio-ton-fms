using BioTonFMS.Infrastructure.MessageBus;

namespace BiotonFMS.Telematica.Tests.Mocks;

public class MessageBusMock : IMessageBus
{
    public readonly List<byte[]> Messages = new();
    
    public void Publish(byte[] message)
    {
        Messages.Add(message);
    }

    public void Subscribe<TBusMessageHandler>() where TBusMessageHandler : IBusMessageHandler
    {
        throw new NotImplementedException();
    }
}