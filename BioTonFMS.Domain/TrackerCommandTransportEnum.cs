using System.ComponentModel;

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Способ доставки команды на трекер
    /// </summary>
    public enum TrackerCommandTransportEnum
    {
        /// <summary>
        /// По сети через TCP/IP
        /// </summary>
        [Description("По сети через TCP/IP")]
        TCP = 1,
        /// <summary>
        /// Через SMS сервис
        /// </summary>
        [Description("Через SMS сервис")]
        SMS = 2
    }
}
