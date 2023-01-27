using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Тип датчика
    /// </summary>
    public sealed class SensorType : EntityBase, IAggregateRoot
    {
        public SensorType(int id, int sensorGroupId, string name, string description, SensorDataTypeEnum? dataType, int? unitId)
        {
            Id = id;
            SensorGroupId = sensorGroupId;
            Name = name;
            Description = description;
            DataType = dataType;
            UnitId = unitId;
        }

        /// <summary>
        /// Название типа датчика
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";
        
        /// <summary>
        /// Описание типа датчика
        /// </summary>
        [MaxLength(2048)]
        public string Description { get; set; } = "";

        /// <summary>
        /// Идентификатор группы датчиков, к которой принадлежат датчики данного типа.
        /// </summary>
        [Required]
        public int SensorGroupId { get; set; }

        /// <summary>
        /// Группа датчиков, к которой принадлежат датчики данного типа.
        /// </summary>
        public SensorGroup SensorGroup { get; set; }

        /// <summary>
        /// Идентификатор типа данных датчиков данного типа. Если не указан, то датчики могут иметь любые типы.
        /// </summary>
        public SensorDataTypeEnum? DataType { get; set; }

        /// <summary>
        /// Идентификатор единицы измерения для датчиков данного типа. Если не указан, то датчики могут иметь любые единицы измерения.
        /// </summary>
        public int? UnitId { get; set; }
        
        /// <summary>
        /// Единица измерения для датчиков данного типа. Если не указана, то датчики могут иметь любые единицы измерения.
        /// </summary>        
        public Unit? Unit { get; set; }

 /// <summary>
        /// Сенсоры данного типа
        /// </summary>
        public List<Sensor> Sensors;
    }
}