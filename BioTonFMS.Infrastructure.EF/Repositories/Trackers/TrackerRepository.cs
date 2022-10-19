using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Paging.Extensions;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BioTonFMS.Infrastructure.Utils.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BioTonFMS.Infrastructure.EF.Repositories.Trackers
{
    public class TrackerRepository : Repository<Tracker>, ITrackerRepository
    {
        private readonly ILogger<TrackerRepository> _logger;
        private readonly IQueryableProvider<Vehicle> _vehicleQueryableProvider;

        public TrackerRepository(
            ILogger<TrackerRepository> logger,
            IQueryableProvider<Vehicle> vehicleQueryableProvider,
            IKeyValueProvider<Tracker, int> keyValueProvider,
            IQueryableProvider<Tracker> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
            _vehicleQueryableProvider = vehicleQueryableProvider;
            _logger = logger;
        }

        public void AddTracker(Tracker tracker)
        {
            var trackersWithTheSameExternalId = QueryableProvider.Linq().Where(t => t.ExternalId == tracker.ExternalId);
            if (trackersWithTheSameExternalId.Any())
            {
                throw new ArgumentException($"Трекер с внешним идентификатором {tracker.ExternalId} уже существует");
            }
            try
            {
                Put(tracker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении трекера {@Tracker}", tracker);
                throw;
            }
        }

        public void UpdateTracker(Tracker tracker)
        {
            var trackersWithTheSameExternalId = QueryableProvider.Linq()
                .Where(t => t.ExternalId == tracker.ExternalId && t.Id != tracker.Id);

            if (trackersWithTheSameExternalId.Any())
            {
                throw new ArgumentException($"Трекер с внешним идентификатором " +
                    $"{tracker.ExternalId} уже существует");
            }

            try
            {
                Update(tracker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении трекера {@id}", tracker.Id);
                throw;
            }
        }

        public void DeleteTracker(Tracker tracker)
        {
            var vehicle = _vehicleQueryableProvider
                .Linq().Where(v => v.TrackerId == tracker.Id).FirstOrDefault();

            if (vehicle is not null)
            {
                _logger.LogError($"Нельзя удалить трекер (id - {tracker.Id}) " +
                   $"привязанный к машине (id - {vehicle.Id})!");
                throw new ArgumentException("Нельзя удалить трекер привязанный к машине");
            }

            try
            {
                Remove(tracker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении трекера {@id}", tracker.Id);
                throw;
            }
        }

        public PagedResult<Tracker> GetTrackers(TrackersFilter filter)
        {
            var linqProvider = QueryableProvider.Linq();

            Expression<Func<Tracker, bool>>? trackerPredicate = null;

            if (filter.ExternalId.HasValue)
            {
                trackerPredicate = tracker => tracker.ExternalId == filter.ExternalId;
            }

            if (filter.Type.HasValue)
            {
                Expression<Func<Tracker, bool>>? typePredicate =
                    tracker => tracker.TrackerType == filter.Type;

                trackerPredicate = SetPredicate(trackerPredicate, typePredicate);
            }

            if (!string.IsNullOrEmpty(filter.SimNumber))
            {
                Expression<Func<Tracker, bool>>? simNumberPredicate =
                   tracker => tracker.SimNumber == filter.SimNumber;

                trackerPredicate = SetPredicate(trackerPredicate, simNumberPredicate);
            }

            var trackers = trackerPredicate != null ?
                linqProvider.Where(trackerPredicate) :
                linqProvider;

            if (filter.SortBy.HasValue && filter.SortBy == TrackerSortBy.StartDate)
            {
                switch (filter.SortDirection)
                {
                    case SortDirection.Ascending:
                        trackers = trackers.OrderBy(s => s.StartDate);
                        break;
                    case SortDirection.Descending:
                        trackers = trackers.OrderByDescending(s => s.StartDate);
                        break;
                }
            }

            return trackers.AsNoTracking().GetPagedQueryable(
                 filter.PageNum, filter.PageSize);
        }

        private static Expression<Func<Tracker, bool>>? SetPredicate(
            Expression<Func<Tracker, bool>>? trackerPredicate,
            Expression<Func<Tracker, bool>> customPredicate)
        {
            if (trackerPredicate == null)
            {
                trackerPredicate = customPredicate;
            }
            else
            {
                trackerPredicate = trackerPredicate?.And(customPredicate);
            }

            return trackerPredicate;
        }
    }
}
