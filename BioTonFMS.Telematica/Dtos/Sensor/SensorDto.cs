using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Extensions;
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Модель датчика
    /// </summary>
    public class SensorDto
    {
        /// <summary>
        /// Id датчика
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Родительский трекер
        /// </summary>
        public ForeignKeyValue<int, string> Tracker { get; set; }

        /// <summary>
        /// Название датчика
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Датчик скрыт
        /// </summary>
        public bool IsHidden { get; set; }
        
        /// <summary>
        /// Тип данных
        /// </summary>
        public SensorDataTypeEnum DataType { get; set; }
        
        /// <summary>
        /// Тип сенсора
        /// </summary>
        public ForeignKeyValue<int, string> SensorType { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Формула по которой рассчитывается значение датчика
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Единица измерения датчика
        /// </summary>
        public ForeignKeyValue<int, string> Unit { get; set; }

        /// <summary>
        /// Использовать последние принятые от трекера значения 
        /// </summary>
        public bool UseLastReceived { get; set; }

        /// <summary>
        /// Валидатор используется для модификации значения датчика  
        /// </summary>
        public ForeignKeyValue<int, string>? Validator { get; set; }

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
