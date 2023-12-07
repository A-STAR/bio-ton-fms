using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;

public class TrackerCommandRepository : Repository<TrackerCommand, BioTonDBContext>, ITrackerCommandRepository
{
    public TrackerCommandRepository(IKeyValueProvider<TrackerCommand, int> keyValueProvider,
        IQueryableProvider<TrackerCommand> queryableProvider,
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactory)
        : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
    }

    public TrackerCommand? GetWithoutCaching(int key)
    {
        TrackerCommand? trackerCommand = QueryableProvider.Linq()
            .AsNoTracking().SingleOrDefault(t => t.Id == key);
        return trackerCommand;
    }

    public ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = end.ToUniversalTime();
        IQueryable<TrackerCommand> commands = QueryableProvider.Linq()
            .AsNoTracking()
            .Where(x => x.Tracker != null &&
                        x.Tracker.ExternalId == externalId &&
                        x.SentDateTime >= startUtc &&
                        x.SentDateTime <= endUtc);

        return new ViewMessageStatisticsDto
        {
            NumberOfMessages = commands.Count(),
            TotalTime = (commands.Select(x => x.SentDateTime).DefaultIfEmpty().Max() -
                         commands.Select(x => x.SentDateTime).DefaultIfEmpty().Min()).Seconds
        };
    }
}