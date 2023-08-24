using System.Net.Sockets;

namespace BioTonFMS.Infrastructure.Utils.Network;

public class FmsTcpClient : ITcpClient
{
    private readonly TcpClient _client;
    private DateTime _lastUsed = DateTime.Now;
    
    public FmsTcpClient()
    {
        _client = new TcpClient();
    }

    public async Task ConnectAsync(string host, int port)
    {
        await _client.ConnectAsync(host, port);
    }

    public Stream GetStream()
    {
        _lastUsed = DateTime.Now;
        return _client.GetStream();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public bool IsConnected()
    {
        return _client.Connected;
    }

    public DateTime LastUsed()
    {
        return _lastUsed;
    }
}