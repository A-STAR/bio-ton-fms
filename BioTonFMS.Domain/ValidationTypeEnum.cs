using System.ComponentModel;

namespace BioTonFMS.Domain;

/// <summary>
/// Типы валидации значения датчика
/// </summary>
public enum ValidationTypeEnum
{
    /// <summary>
    /// Тип валидации «логическое И»
    /// </summary>
    [Description("Тип валидации «Логическое И»")]
    LogicalAnd,

    /// <summary>
    /// Тип валидации «логическое ИЛИ»
    /// </summary>
    [Description("Тип валидации «Логическое ИЛИ»")]
    LogicalOr,

    /// <summary>
    /// Тип валидации «сравнение с нулём»
    /// </summary>
    [Description("Тип валидации «Проверка на неравенство нулю»")]
    ZeroTest
}
