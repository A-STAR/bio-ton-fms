using System.Linq.Expressions;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Paging.Extensions;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BioTonFMS.Infrastructure.Utils.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Infrastructure.EF.Repositories.Trackers
{
    public class TrackerRepository : Repository<Tracker, BioTonDBContext>, ITrackerRepository
    {
        private readonly ILogger<TrackerRepository> _logger;
        private readonly IQueryableProvider<Vehicle> _vehicleQueryableProvider;

        public TrackerRepository(
            ILogger<TrackerRepository> logger,
            IQueryableProvider<Vehicle> vehicleQueryableProvider,
            IKeyValueProvider<Tracker, int> keyValueProvider,
            IQueryableProvider<Tracker> queryableProvider,
            UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
            _vehicleQueryableProvider = vehicleQueryableProvider;
            _logger = logger;
        }        
        
        private IQueryable<Tracker> HydratedQuery
        {
            get
            {

                var linqProvider = QueryableProvider
                    .Fetch(x => x.Vehicle)
                    .Fetch(x => x.Sensors)
                    .Linq();
                return linqProvider;
            }
        }

        public new Tracker? this[int key] => HydratedQuery.FirstOrDefault(x => x.Id == key);

        public override void Add(Tracker tracker)
        {
            var trackersWithTheSameExternalId = QueryableProvider.Linq().Where(t => t.ExternalId == tracker.ExternalId);
            if (trackersWithTheSameExternalId.Any())
            {
                throw new ArgumentException($"Трекер с внешним идентификатором {tracker.ExternalId} уже существует");
            }
            try
            {
                base.Add(tracker);
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "Ошибка при добавлении трекера {@Tracker}", tracker);
                throw;
            }
        }

        public override void Update(Tracker tracker)
        {
            var trackersWithTheSameExternalId = QueryableProvider.Linq()
                .Where(t => t.ExternalId == tracker.ExternalId && t.Id != tracker.Id);

            if (trackersWithTheSameExternalId.Any())
            {
                throw new ArgumentException("Трекер с внешним идентификатором " +
                    $"{tracker.ExternalId} уже существует");
            }

            try
            {
                base.Update(tracker);
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "Ошибка при обновлении трекера {TrackerId}", tracker.Id);
                throw;
            }
        }

        public override void Remove(Tracker tracker)
        {
            var vehicle = _vehicleQueryableProvider.Linq().FirstOrDefault(v => v.TrackerId == tracker.Id);

            if (vehicle is not null)
            {
                var regNum = vehicle.RegistrationNumber.Length > 0
                    ? vehicle.RegistrationNumber
                    : "не заполнен";
                _logger.LogError("Нельзя удалить трекер (id - {TrackerId}) привязанный к машине (id - {VehicleId})!",
                    tracker.Id, vehicle.Id);
                throw new ArgumentException($"Нельзя удалить трекер привязанный к машине (название - '{vehicle.Name}', " +
                                            $"регистрационный номер - {regNum})");
            }

            try
            {
                base.Remove(tracker);
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "Ошибка при удалении трекера {@id}", tracker.Id);
                throw;
            }
        }

        public PagedResult<Tracker> GetTrackers(TrackersFilter filter, bool forUpdate = false)
        {
            var trackers = forUpdate ? HydratedQuery : HydratedQuery.AsNoTracking();

            Expression<Func<Tracker, bool>>? trackerPredicate = null;

            if (filter.ExternalId.HasValue)
            {
                trackerPredicate = tracker => tracker.ExternalId == filter.ExternalId;
            }

            if (filter.Imei is not null)
            {
                trackerPredicate = tracker => tracker.Imei == filter.Imei;
            }

            if (filter.Type.HasValue)
            {
                Expression<Func<Tracker, bool>> typePredicate =
                    tracker => tracker.TrackerType == filter.Type;

                trackerPredicate = SetPredicate(trackerPredicate, typePredicate);
            }

            if (!string.IsNullOrEmpty(filter.SimNumber))
            {
                Expression<Func<Tracker, bool>> simNumberPredicate =
                    tracker => tracker.SimNumber == filter.SimNumber;

                trackerPredicate = SetPredicate(trackerPredicate, simNumberPredicate);
            }

            trackers = trackerPredicate is not null
                ? trackers.Where(trackerPredicate)
                : trackers;

            trackers = filter.SortBy switch
            {
                TrackerSortBy.Name => trackers.SetSortDirection(filter.SortDirection, x => x.Name),
                TrackerSortBy.ExternalId => trackers.SetSortDirection(filter.SortDirection, x => x.ExternalId),
                TrackerSortBy.Type => trackers.SetSortDirection(filter.SortDirection, x => x.TrackerType),
                TrackerSortBy.SimNumber => trackers.SetSortDirection(filter.SortDirection, x => x.SimNumber),
                TrackerSortBy.Vehicle => trackers.SetSortDirection(filter.SortDirection,
                    x => x.Vehicle == null ? "" : x.Vehicle.Name),
                TrackerSortBy.StartDate => trackers.SetSortDirection(filter.SortDirection, x => x.StartDate),
                _ => trackers
            };

            return trackers.AsNoTracking().GetPagedQueryable(
                filter.PageNum, filter.PageSize);
        }

        private static Expression<Func<Tracker, bool>> SetPredicate(
            Expression<Func<Tracker, bool>>? trackerPredicate,
            Expression<Func<Tracker, bool>> customPredicate)
        {
            trackerPredicate = trackerPredicate == null ? customPredicate : trackerPredicate.And(customPredicate);

            return trackerPredicate;
        }
    }
}
