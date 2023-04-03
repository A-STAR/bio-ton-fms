using BioTonFMS.Domain;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos.TrackerCommand
{
    /// <summary>
    /// Команда для трекера
    /// </summary>
    public class TrackerCommandDto
    {
        /// <summary>
        /// Cтрока команды, строка включает в себя параметры команды если требуется
        /// </summary>
        [Required]
        public string CommandText { get; set; } = string.Empty;

        /// <summary>
        /// Способ доставки команды на трекер
        /// </summary>
        [Required]
        public TrackerCommandTransportEnum Transport { get; set; } = TrackerCommandTransportEnum.TCP;
    }
}
