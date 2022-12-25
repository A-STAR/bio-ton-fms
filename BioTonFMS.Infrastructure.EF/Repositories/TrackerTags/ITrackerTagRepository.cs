using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

public interface ITrackerTagRepository : IRepository<TrackerTag>
{
    /// <summary>
    /// Возвращает список тегов для заданного типа трекера.
    /// </summary>
    IEnumerable<TagDto> GetTagsForTrackerType(TrackerTypeEnum trackerType);

    /// <summary>
    /// Возвращает тег для заданного протокола trackerType,
    /// определяемый кодом protocolTagCode, если маппинг определён или null.
    /// </summary>
    TagDto? GetTagForTrackerType(TrackerTypeEnum trackerType, int protocolTagCode);
}

// Нужно перенести куда-нибудь
public class TagDto
{
    /// <summary>
    /// Идентификатор тега
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Код тега в протоколе
    /// </summary>
    public int ProtocolTagCode { get; set; }

    /// <summary>
    /// Название тега для отображения в системе
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Тип данных тега
    /// </summary>
    public TagDataTypeEnum DataType { get; set; }

    /// <summary>
    /// Тип структуры (определяет способ интерпретации данных в структуре)
    /// </summary>
    public StructTypeEnum? StructType { get; set; }

    /// <summary>
    /// Тип трекера (определяет протокол)
    /// </summary>
    public TrackerTypeEnum TrackerType { get; set; }

    /// <summary>
    /// Описание тега
    /// </summary>
    public string? Description { get; set; }
}