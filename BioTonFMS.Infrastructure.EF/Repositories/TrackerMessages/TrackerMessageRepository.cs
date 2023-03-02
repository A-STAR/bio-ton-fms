using BioTonFMS.Common.Testable;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Providers;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;

public class TrackerMessageRepository : Repository<TrackerMessage, MessagesDBContext>, ITrackerMessageRepository
{
    public TrackerMessageRepository(IKeyValueProvider<TrackerMessage, int> keyValueProvider,
        IQueryableProvider<TrackerMessage> queryableProvider,
        UnitOfWorkFactory<MessagesDBContext> unitOfWorkFactory)
        : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
    }

    public override void Add(TrackerMessage entity)
    {
        entity.ServerDateTime = SystemTime.UtcNow;
        base.Add(entity);
    }

    public IList<TrackerMessage> GetMessagesForUpdate()
    {
        var linqProvider = QueryableProvider
            .Fetch(m => m.Tags)
            .Linq()
            .OrderBy(m => m.TrId)
            .ThenBy(m => m.Id);
        return linqProvider.ToList();
    }
    
    public IList<TrackerMessage> GetMessages()
    {
        var linqProvider = QueryableProvider.Fetch(m => m.Tags).Linq().AsNoTracking().OrderBy(m => m.Id);
        return linqProvider.ToList();
    }

    public bool ExistsByUID(Guid uid) =>
        QueryableProvider.Linq().Any(x => x.PackageUID == uid);
}
