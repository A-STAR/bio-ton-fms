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
    /// Уровень топлива, температура охлаждающей жидкости и обороты двигателя
    /// </summary>
    [Description("Уровень топлива, температура охлаждающей жидкости и обороты двигателя")]
    FuelLevel_TempCool_EngineSpeed = 4
}