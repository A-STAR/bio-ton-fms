using System.ComponentModel;

namespace BioTonFMS.Domain;

public enum TagDataTypeEnum : byte
{
    /// <summary>
    /// Целое число
    /// </summary>
    [Description("Целое число")]
    Integer = 1,
    /// <summary>
    /// Набор битов
    /// </summary>
    [Description("Набор битов")]
    Bits = 2,
    /// <summary>
    /// Байт
    /// </summary>
    [Description("Байт")]
    Byte = 3,
    /// <summary>
    /// Вещественное число
    /// </summary>
    [Description("Вещественное число")]
    Double = 4,
    /// <summary>
    /// Логическое
    /// </summary>
    [Description("Логическое")]
    Boolean = 5,
    /// <summary>
    /// Строка
    /// </summary>
    [Description("Строка")]
    String = 6,
    /// <summary>
    /// Дата и время
    /// </summary>
    [Description("Дата и время")]
    DateTime = 7,
    /// <summary>
    /// Структура
    /// </summary>
    [Description("Структура")]
    Struct = 8
}