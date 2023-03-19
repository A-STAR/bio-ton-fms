using System.ComponentModel.DataAnnotations;
using BioTonFMS.Infrastructure.Models;

namespace BioTonFMS.Domain;

/// <summary>
/// Структура для хранения маппинга тегов на  протокол
/// </summary>
public class ProtocolTag : EntityBase, IAggregateRoot
{
    /// <summary>
    /// Тип трекера (определяет протокол)
    /// </summary>
    [Required]
    public TrackerTypeEnum TrackerType { get; set; }

    /// <summary>
    /// Идентификатор тега
    /// </summary>
    [Required]
    public int TagId { get; set; }

    /// <summary>
    /// Ссылка на тег
    /// </summary>
    [Required]
    public TrackerTag? Tag { get; set; } = null!;

    /// <summary>
    /// Код тега в протоколе
    /// </summary>
    [Required]
    public int ProtocolTagCode { get; set; }

    /// <summary>
    /// Размер тега в байтах
    /// </summary>
    [Required]
    public int Size { get; set; }
}