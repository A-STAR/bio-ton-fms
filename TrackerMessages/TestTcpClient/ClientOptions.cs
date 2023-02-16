namespace TestTcpTrackerClient;

public class ClientOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6000;
    public int DelaySeconds { get; set; } = 10;
}