namespace BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;

public interface IProtocolMessageHandler
{
    byte[] HandleMessage(byte[] message);
    int GetPacketLength(byte[] message);
}