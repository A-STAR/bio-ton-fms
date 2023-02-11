using BioTonFMS.Domain;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos.Tracker
{
    /// <summary>
    /// Модель создания трекера
    /// </summary>
    public class CreateTrackerDto
    {
        /// <summary>
        /// Идентификатор трекера во внешней системе
        /// </summary>
        [Required]
        public int ExternalId { get; set; }

        /// <summary>
        /// Название трекера
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Номер sim – карты
        /// </summary>
        [Required]
        public string SimNumber { get; set; } = string.Empty;

        /// <summary>
        /// IMEI трекера
        /// </summary>
        public string Imei { get; set; } = string.Empty;

        /// <summary>
        /// Тип устройства
        /// </summary>
        [EnumDataType(typeof(TrackerTypeEnum))]
        public TrackerTypeEnum TrackerType { get; set; }

        /// <summary>
        /// Дата и время начала действия трекера на данной машине
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
