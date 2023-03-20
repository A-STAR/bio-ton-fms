using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;

public interface ITrackerMessageRepository : IRepository<TrackerMessage>
{
    bool ExistsByUID(Guid uid);
    IList<TrackerMessage> GetMessagesForUpdate();
    IList<TrackerMessage> GetMessages();
    TrackerStandardParameters GetStandardParameters(int externalId, string imei);
    IList<TrackerParameter> GetParameters(int externalId, string imei);
    PagedResult<ParametersHistoryRecord> GetParametersHistory(ParametersHistoryFilter filter);
}