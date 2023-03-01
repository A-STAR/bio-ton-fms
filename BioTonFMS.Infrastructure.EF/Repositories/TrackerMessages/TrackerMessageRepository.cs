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

    public TrackerStandardParameters GetParameters(int trackerId)
    {
        var stdParams = new TrackerStandardParameters();
        var last = QueryableProvider.Linq()
            .Where(x => x.TrId == trackerId)
            .MaxBy(x => x.ServerDateTime);

        if (last == null) return stdParams;

        stdParams.Time = last.ServerDateTime;
        
        stdParams.Long = last.Longitude ?? QueryableProvider.Linq()
            .Where(x => x.Longitude != null && x.TrId == trackerId)
            .MaxBy(x => x.ServerDateTime)?.Longitude;

        stdParams.Lat = last.Latitude ?? QueryableProvider.Linq()
            .Where(x => x.Latitude != null && x.TrId == trackerId)
            .MaxBy(x => x.ServerDateTime)?.Latitude;

        stdParams.Speed = last.Speed ?? QueryableProvider.Linq()
            .Where(x => x.Speed != null && x.TrId == trackerId)
            .MaxBy(x => x.ServerDateTime)?.Speed;

        stdParams.Alt = last.Height ?? QueryableProvider.Linq()
            .Where(x => x.Height != null && x.TrId == trackerId)
            .MaxBy(x => x.ServerDateTime)?.Height;

        return stdParams;
    }
}