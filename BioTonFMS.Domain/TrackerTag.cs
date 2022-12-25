using System.ComponentModel.DataAnnotations;
using BioTonFMS.Infrastructure.Models;

namespace BioTonFMS.Domain;

/// <summary>
/// Cтруктура для хранения тегов, используемых при разборе сообщений от трекеров
/// </summary>
public class TrackerTag : EntityBase, IAggregateRoot
{
    /// <summary>
    /// Название тега для отображения в системе
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = "";

    /// <summary>
    /// Тип данных тега
    /// </summary>
    [Required]
    public TagDataTypeEnum DataType { get; set; }

    /// <summary>
    /// Тип структуры (определяет способ интерпретации данных в структуре)
    /// </summary>
    public StructTypeEnum? StructType { get; set; }

    /// <summary>
    /// Описание тега
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Ссылка на тег протокола
    /// </summary>
    [Required]
    public ProtocolTag ProtocolTag { get; set; }
}