using System.Net;

namespace BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;

public interface IProtocolMessageHandler
{
    Task<byte[]> HandleMessage(byte[] message, IPAddress ip, int port);
    int GetPacketLength(byte[] message);
}