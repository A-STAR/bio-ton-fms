using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Датчик
    /// </summary>
    public class Sensor : EntityBase, IAggregateRoot
    {

        /// <summary>
        /// Идентификатор родительского трекера
        /// </summary>
        [Required]
        public int TrackerId { get; set; }
        
        /// <summary>
        /// Родительский трекер
        /// </summary>
        public Tracker Tracker { get; set; }

        /// <summary>
        /// Название датчика
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";
        
        /// <summary>
        /// Датчик видим
        /// </summary>
        public bool IsVisible { get; set; }
        
        /// <summary>
        /// Тип данных
        /// </summary>
        [Required]
        public SensorDataTypeEnum DataType { get; set; }
        
        /// <summary>
        /// Идентификатор типа сенсора
        /// </summary>
        [Required]
        public int SensorTypeId { get; set; }

        /// <summary>
        /// Тип сенсора
        /// </summary>
        public SensorType SensorType { get; set; }

        /// <summary>
        /// Описание датчика
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = "";
        
        /// <summary>
        /// Формула по которой рассчитывается значение датчика
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Formula { get; set; }

        /// <summary>
        /// Идентификатор единицы измерения датчика
        /// </summary>
        [Required]
        public int UnitId { get; set; }

        /// <summary>
        /// Единица измерения датчика
        /// </summary>
        public Unit Unit { get; set; }

        /// <summary>
        /// Использовать последние принятые от трекера значения 
        /// </summary>
        [Required]
        public bool UseLastReceived { get; set; } = false;

        /// <summary>
        /// Валидатор используется для модификации значения датчика  
        /// </summary>
        public int? ValidatorId { get; set; }
        [ForeignKey("ValidatorId")]
        public Sensor? Validator { get; set; }

        /// <summary>
        /// Тип валидации
        /// </summary>
        public ValidationTypeEnum? ValidationType { get; set; }

        /// <summary>
        /// Использование горючего (л/c)
        /// </summary>
        public double? FuelUse { get; set; }
    }
}