using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Модель группы машин
    /// </summary>ы
    public class VehicleGroupDto
    {
        /// <summary>
        /// Id группы машин
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название группы машин
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
