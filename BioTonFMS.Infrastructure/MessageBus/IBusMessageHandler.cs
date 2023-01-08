namespace BioTonFMS.Infrastructure.MessageBus
{
    public interface IBusMessageHandler
    {
        Task HandleAsync(byte[] message);
    }
}
