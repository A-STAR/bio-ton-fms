using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;

public interface ITrackerMessageRepository : IRepository<TrackerMessage>
{
    bool ExistsByUid(Guid uid);
    IList<TrackerMessage> GetMessages(bool forUpdate = false);
    TrackerStandardParameters GetStandardParameters(int externalId, string imei);
    IList<TrackerParameter> GetParameters(int externalId, string imei);
    IDictionary<int, TrackerMessage> GetLastMessagesFor(ICollection<int> externalTrackerIds);
    PagedResult<ParametersHistoryRecord> GetParametersHistory(ParametersHistoryFilter filter);
}