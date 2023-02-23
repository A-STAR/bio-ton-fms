using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;

public interface ITrackerMessageRepository : IRepository<TrackerMessage>
{
    IList<TrackerMessage> GetMessagesForUpdate();
    IList<TrackerMessage> GetMessages();
}