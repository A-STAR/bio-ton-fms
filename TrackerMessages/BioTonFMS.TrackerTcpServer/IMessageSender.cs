namespace BioTonFMS.TrackerTcpServer
{
    public interface IMessageSender
    {
        void Send(byte[] message);
    }
}
