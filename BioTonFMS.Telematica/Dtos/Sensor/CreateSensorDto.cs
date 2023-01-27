using System.ComponentModel.DataAnnotations;
using BioTonFMS.Domain;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Модель создания датчика
    /// </summary>
    public class CreateSensorDto
    {
        /// <summary>
        /// Идентификатор родительского трекера
        /// </summary>
        [Required]
        public int TrackerId { get; set; }

        /// <summary>
        /// Название датчика
        /// </summary>
        [Required]
        public string Name { get; set; } = "";
        
        /// <summary>
        /// Тип данных
        /// </summary>
        [Required]
        public SensorDataTypeEnum DataType { get; set; }
        
        /// <summary>
        /// Тип сенсора
        /// </summary>
        [Required]
        public int SensorTypeId { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; } = "";
        
        /// <summary>
        /// Формула по которой рассчитывается значение датчика
        /// </summary>
        [Required]
        public string Formula { get; set; }

        /// <summary>
        /// Идентификатор единицы измерения датчика
        /// </summary>
        [Required]
        public int UnitId { get; set; }

        /// <summary>
        /// Использовать последние принятые от трекера значения 
        /// </summary>
        [Required]
        public bool UseLastReceived { get; set; } = false;

        /// <summary>
        /// Валидатор используется для модификации значения датчика  
        /// </summary>
        public int? ValidatorId { get; set; }

        /// <summary>
        /// Тип валидации
        /// </summary>
        public ValidationTypeEnum? ValidationType { get; set; }

        /// <summary>
        /// Использование горючего (л/c)
        /// </summary>
        public float FuelUse { get; set; }
    }
}
