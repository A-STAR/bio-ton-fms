namespace BioTonFMS.TrackerMessageHandler.Retranslation;

public class RetranslatorOptions
{
    public bool Enabled { get; set; } = false;
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6000;
    public int TimeoutSeconds { get; set; } = 5;
    public int TcpClientMaxIdleTimeSeconds { get; set; } = 10 * 60;
    public string[]? AllowedExtIds { get; set; }
}