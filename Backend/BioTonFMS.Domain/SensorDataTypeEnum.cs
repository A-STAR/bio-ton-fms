using System.ComponentModel;

namespace BioTonFMS.Domain;

/// <summary>
/// Типы данных сенсоров
/// </summary>
public enum SensorDataTypeEnum
{
    /// <summary>
    /// Булев тип
    /// </summary>
    [Description("Булево")]
    Boolean,

    /// <summary>
    /// Тип число
    /// </summary>
    [Description("Число")]
    Number,
    
    /// <summary>
    /// Тип строка
    /// </summary>
    [Description("Строка")]
    String,
}
