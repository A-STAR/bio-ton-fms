using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Paging.Extensions;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Infrastructure.EF.Repositories.Sensors
{
    public class SensorRepository : Repository<Sensor, BioTonDBContext>, ISensorRepository
    {
        private readonly ILogger<SensorRepository> _logger;

        public SensorRepository(
            ILogger<SensorRepository> logger,
            IKeyValueProvider<Sensor, int> keyValueProvider,
            IQueryableProvider<Sensor> queryableProvider,
            UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
            _logger = logger;
        }

        public IQueryable<Sensor> HydratedQuery =>
            QueryableProvider
                .Fetch(v => v.Tracker)
                .Fetch(v => v.Validator)
                .Fetch(v => v.SensorType)
                .Fetch(v => v.Unit)
                .Linq();

        public new Sensor? this[int id] => HydratedQuery.SingleOrDefault(s => s.Id == id);

        public override void Add(Sensor sensor)
        {
            var sameNameRecord = QueryableProvider.Linq().Where(v => v.Name == sensor.Name && v.TrackerId == sensor.TrackerId);
            if (sameNameRecord.Any())
            {
                throw new ArgumentException($"Датчик с именем \"{sensor.Name}\" уже существует!");
            }

            try
            {
                base.Add(sensor);
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "Ошибка при добавлении датчика {@Sensor}", sensor);
                throw;
            }
        }

        public override void Update(Sensor sensor)
        {
            try
            {
                base.Update(sensor);
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "Ошибка при обновлении датчика {@id}", sensor.Id);
                throw;
            }
        }

        public override void Remove(Sensor sensor)
        {
            if (QueryableProvider.Linq().Any(v => v.ValidatorId == sensor.Id))
            {
                throw new ArgumentException("Невозможно удалить датчик, так как он используется в качестве валидатора в других датчиках!");
            }

            try
            {
                base.Remove(sensor);
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "Ошибка при удалении датчика {@id}", sensor.Id);
                throw;
            }
        }

        public PagedResult<Sensor> GetSensors(SensorsFilter filter)
        {
            var query = HydratedQuery;

            query = filter.TrackerId.HasValue ? query.Where(sensor => sensor.TrackerId == filter.TrackerId) : query;

            if (filter.SortBy is SensorSortBy.Name)
            {
                query = filter.SortDirection switch
                {
                    SortDirection.Ascending => query.OrderBy(s => s.Name),
                    SortDirection.Descending => query.OrderByDescending(s => s.Name),
                    _ => query.OrderBy(s => s.Name)
                };
            }
            else
            {
                query = query.OrderBy(s => s.Id);
            }

            return query.AsNoTracking().GetPagedQueryable(filter.PageNum, filter.PageSize);
        }
    }
}
