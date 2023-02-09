using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Тип топлива
    /// </summary>
    public class FuelType : EntityBase, IAggregateRoot
    {
        /// <summary>
        /// Название типа топлива
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = "";
    }
}