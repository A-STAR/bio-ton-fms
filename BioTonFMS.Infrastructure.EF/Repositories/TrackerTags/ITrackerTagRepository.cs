using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

public interface ITrackerTagRepository : IRepository<TrackerTag>
{
    /// <summary>
    /// Возвращает список тегов для заданного типа трекера.
    /// </summary>
    IEnumerable<TrackerTag> GetTagsForTrackerType(TrackerTypeEnum trackerType);

    /// <summary>
    /// Возвращает тег для заданного протокола trackerType,
    /// определяемый кодом protocolTagCode, если маппинг определён или null.
    /// </summary>
    TrackerTag? GetTagForTrackerType(TrackerTypeEnum trackerType, int protocolTagCode);
}