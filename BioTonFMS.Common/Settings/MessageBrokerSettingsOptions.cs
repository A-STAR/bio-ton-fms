namespace BioTonFMS.Common.Settings
{
    public class MessageBrokerSettingsOptions
    {
        public bool Enabled { get; set; } = true;
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; } = 15672;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;
    }
}
