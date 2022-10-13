using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Группа машин
    /// </summary>
    public class VehicleGroup : EntityBase, IAggregateRoot
    {
        /// <summary>
        /// Название группы
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = "";
    }
}