using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Единица измерения
    /// </summary>
    public sealed class Unit : EntityBase, IAggregateRoot
    {
        public Unit(int id, string name, string abbreviated)
        {
            Id = id;
            Name = name;
            Abbreviated = abbreviated;
        }

        /// <summary>
        /// Название единицы измерения
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// Сокращенное название единицы измерения, которое пишется после значения
        /// </summary>
        [MaxLength(15)]
        public string Abbreviated { get; set; }
    }
}