using System.ComponentModel;

namespace BioTonFMS.Domain.Monitoring
{
    /// <summary>
    /// Перечисление состояний соединения
    /// </summary>
    public enum ConnectionStatusEnum
    {
        /// <summary>
        /// Не подключен
        /// </summary>
        [Description("Не подключен")]
        NotConnected = 0,

        /// <summary>
        /// Подключен
        /// </summary>
        [Description("Подключен")]
        Connected = 1
    }
}
