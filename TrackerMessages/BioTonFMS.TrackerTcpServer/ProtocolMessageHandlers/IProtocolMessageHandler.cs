using System.Net;

namespace BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;

public interface IProtocolMessageHandler
{
    byte[] HandleMessage(byte[] message, IPAddress ip, int port);
    int GetPacketLength(byte[] message);
}