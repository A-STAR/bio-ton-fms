using BioTonFMS.Common.Testable;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;

public class TrackerMessageRepository : Repository<TrackerMessage, MessagesDBContext>, ITrackerMessageRepository
{
    public TrackerMessageRepository(IKeyValueProvider<TrackerMessage, int> keyValueProvider,
        IQueryableProvider<TrackerMessage> queryableProvider,
        UnitOfWorkFactory<MessagesDBContext> unitOfWorkFactory) : base(keyValueProvider, queryableProvider,
        unitOfWorkFactory)
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

    public TrackerStandardParameters GetParameters(int id)
    {
        var prs = new TrackerStandardParameters();
        var last = QueryableProvider.Linq()
            .Where(x => x.TrId == id)
            .MaxBy(x => x.ServerDateTime);

        if (last == null) return prs;

        prs.Time = last.ServerDateTime;
        
        prs.Long = last.Longitude ?? QueryableProvider.Linq()
            .Where(x => x.Longitude != null && x.TrId == id)
            .MaxBy(x => x.ServerDateTime)?.Longitude;

        prs.Lat = last.Latitude ?? QueryableProvider.Linq()
            .Where(x => x.Latitude != null && x.TrId == id)
            .MaxBy(x => x.ServerDateTime)?.Latitude;

        prs.Speed = last.Speed ?? QueryableProvider.Linq()
            .Where(x => x.Speed != null && x.TrId == id)
            .MaxBy(x => x.ServerDateTime)?.Speed;

        prs.Alt = last.Height ?? QueryableProvider.Linq()
            .Where(x => x.Height != null && x.TrId == id)
            .MaxBy(x => x.ServerDateTime)?.Height;

        return prs;
    }
}