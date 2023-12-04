using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;

public interface ITrackerCommandRepository : IRepository<TrackerCommand>
{
    TrackerCommand? GetWithoutCaching(int key);
    ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end);
}