using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Группа датчиков
    /// </summary>
    public sealed class SensorGroup : EntityBase, IAggregateRoot
    {
        public SensorGroup(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Название группы датчиков
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// Описание группы датчиков
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = "";

        /// <summary>
        /// Типы датчиков
        /// </summary>
        public List<SensorType> SensorTypes { get; set; }
    }
}