using System.ComponentModel;

namespace BioTonFMS.Domain.Monitoring
{
    /// <summary>
    /// Перечисление состояний движения
    /// </summary>
    public enum MovementStatusEnum
    {
        /// <summary>
        /// Нет данных
        /// </summary>
        [Description("Нет данных")]
        NoData = 0,

        /// <summary>
        /// В движении
        /// </summary>
        [Description("В движении")]
        Moving = 1,

        /// <summary>
        /// В движении
        /// </summary>
        [Description("Остановка")]
        Stopped = 2
    }
}
