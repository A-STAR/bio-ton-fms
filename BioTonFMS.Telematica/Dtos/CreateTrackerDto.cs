using BioTonFMS.Domain;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos
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
        public string Name { get; set; }

        /// <summary>
        /// Номер sim – карты
        /// </summary>
        [Required]
        public string SimNumber { get; set; }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public TrackerTypeEnum TrackerType { get; set; }

        /// <summary>
        /// Дата и время начала действия трекера на данной машине
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
    }
}
