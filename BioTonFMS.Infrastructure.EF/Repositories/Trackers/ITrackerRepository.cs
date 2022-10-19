using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories
{
    public interface ITrackerRepository : IRepository<Tracker>
    {
        public PagedResult<Tracker> GetTrackers(TrackersFilter filter);
        public void AddTracker(Tracker tracker);
        public void UpdateTracker(Tracker tracker);
        public void DeleteTracker(Tracker tracker);
    }
}
