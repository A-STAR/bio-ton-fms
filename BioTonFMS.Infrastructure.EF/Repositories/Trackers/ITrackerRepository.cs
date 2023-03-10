using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.Trackers
{
    public interface ITrackerRepository : IRepository<Tracker>
    {
        PagedResult<Tracker> GetTrackers(TrackersFilter filter);
    }
}
