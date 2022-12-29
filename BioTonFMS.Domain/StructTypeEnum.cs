using System.ComponentModel;

namespace BioTonFMS.Domain;

/// <summary>
/// Перечисление структурных типов
/// </summary>
public enum StructTypeEnum
{
    /// <summary>
    /// Координаты и их источник
    /// </summary>
    [Description("Координаты и их источник")]
    Coordinates = 1,

    /// <summary>
    /// Скорость и направление
    /// </summary>
    [Description("Скорость и направление")]
    SpeedAndDirection = 2,

    /// <summary>
    /// Ускорение
    /// </summary>
    [Description("Ускорение")]
    Acceleration = 3,
    
    /// <summary>
    /// Уровень топлива
    /// </summary>
    [Description("Уровень топлива")]
    FuelLevel = 4,
    
    /// <summary>
    /// Температура охлаждающей жидкости
    /// </summary>
    [Description("Температура охлаждающей жидкости")]
    TempCool = 5,
    
    /// <summary>
    /// Обороты двигателя
    /// </summary>
    [Description("Обороты двигателя")]
    EngineSpeed = 6
}