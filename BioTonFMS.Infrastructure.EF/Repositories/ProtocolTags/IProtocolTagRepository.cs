using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.ProtocolTags;

public interface IProtocolTagRepository : IRepository<ProtocolTag>
{
    /// <summary>
    /// Возвращает список тегов для заданного типа трекера.
    /// </summary>
    IEnumerable<ProtocolTag> GetTagsForTrackerType(TrackerTypeEnum trackerType);
}