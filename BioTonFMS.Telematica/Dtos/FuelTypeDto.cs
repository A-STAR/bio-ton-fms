using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Модель типа топлива
    /// </summary>
    public class FuelTypeDto
    {
        /// <summary>
        /// Id типа топлива
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название типа топлива
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
