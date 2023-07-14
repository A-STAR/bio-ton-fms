using BioTonFMS.Infrastructure.Utils.Network;

namespace BiotonFMS.Telematica.Tests.Mocks;

public class TcpClientMock : ITcpClient
{
    public readonly Stream MockStream;
    
    public TcpClientMock(Stream s)
    {
        MockStream = s;
    }
    
    public void Dispose()
    {
        MockStream.Dispose();
    }

    public Task ConnectAsync(string host, int port) => Task.CompletedTask;

    public Stream GetStream()
    {
        return MockStream;
    }

    public bool IsConnected()
    {
        throw new NotImplementedException();
    }

    public DateTime LastUsed()
    {
        throw new NotImplementedException();
    }
}