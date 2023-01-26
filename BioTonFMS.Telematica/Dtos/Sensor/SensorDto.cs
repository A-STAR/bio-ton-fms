using BioTonFMS.Domain;
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
        /// Идентификатор родительского трекера
        /// </summary>
        public int TrackerId { get; set; }

        /// <summary>
        /// Название датчика
        /// </summary>
        public string Name { get; set; } = "";
        
        /// <summary>
        /// Тип данных
        /// </summary>
        public SensorDataTypeEnum DataType { get; set; }
        
        /// <summary>
        /// Тип сенсора
        /// </summary>
        public int SensorTypeId { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; } = "";
        
        /// <summary>
        /// Формула по которой рассчитывается значение датчика
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Единица измерения датчика
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// Использовать последние принятые от трекера значения 
        /// </summary>
        public bool UseLastReceived { get; set; } = false;

        /// <summary>
        /// Валидатор используется для модификации значения датчика  
        /// </summary>
        public int ValidatorId { get; set; }

        /// <summary>
        /// Тип валидации
        /// </summary>
        public ValidationTypeEnum ValidationType { get; set; }

        /// <summary>
        /// Использование горючего (л/c)
        /// </summary>
        public float FuelUse { get; set; }
    }
}
