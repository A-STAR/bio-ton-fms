namespace BioTonFMS.Infrastructure.Utils.Network;

public interface ITcpClient : IDisposable
{
    Task ConnectAsync(string host, int port);
    bool IsConnected();
    Stream GetStream();
    DateTime LastUsed();
}