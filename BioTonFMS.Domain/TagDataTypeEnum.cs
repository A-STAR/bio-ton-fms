using System.ComponentModel;

namespace BioTonFMS.Domain;

public enum TagDataTypeEnum
{
    /// <summary>
    /// Целое число
    /// </summary>
    [Description("Целое число")]
    Integer = 1,
    /// <summary>
    /// Вещественное число
    /// </summary>
    [Description("Вещественное число")]
    Double = 2,
    /// <summary>
    /// Логическое
    /// </summary>
    [Description("Логическое")]
    Boolean = 3,
    /// <summary>
    /// Строка
    /// </summary>
    [Description("Строка")]
    String = 4,
    /// <summary>
    /// Дата и время
    /// </summary>
    [Description("Дата и время")]
    DateTime = 5,
    /// <summary>
    /// Набор битов
    /// </summary>
    [Description("Набор битов")]
    Bits = 6,
    /// <summary>
    /// Байт
    /// </summary>
    [Description("Байт")]
    Byte = 7,
    /// <summary>
    /// Структура
    /// </summary>
    [Description("Структура")]
    Struct = 7
}