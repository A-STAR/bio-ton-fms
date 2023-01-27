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
using System.Linq.Expressions;

namespace BioTonFMS.Infrastructure.EF.Repositories.Sensors
{
    public class SensorRepository : Repository<Sensor>, ISensorRepository
    {
        private readonly ILogger<SensorRepository> _logger;

        public SensorRepository(
            ILogger<SensorRepository> logger,
            IKeyValueProvider<Sensor, int> keyValueProvider,
            IQueryableProvider<Sensor> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory) : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
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
            try
            {
                base.Add(sensor);
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении датчика {@id}", sensor.Id);
                throw;
            }
        }

        public override void Remove(Sensor sensor)
        {
            try
            {
                base.Remove(sensor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении датчика {@id}", sensor.Id);
                throw;
            }
        }

        public PagedResult<Sensor> GetSensors(SensorsFilter filter)
        {
            Expression<Func<Sensor, bool>>? sensorPredicate = null;

            if (filter.TrackerId.HasValue)
            {
                sensorPredicate = sensor => sensor.TrackerId == filter.TrackerId;
            }
            
            var sensors = sensorPredicate != null ? HydratedQuery.Where(sensorPredicate) : HydratedQuery;

            if (filter.SortBy is not SensorSortBy.Name)
                return sensors.AsNoTracking().GetPagedQueryable(
                    filter.PageNum, filter.PageSize);

            sensors = filter.SortDirection switch
            {
                SortDirection.Ascending => sensors.OrderBy(s => s.Name),
                SortDirection.Descending => sensors.OrderByDescending(s => s.Name),
                _ => sensors
            };

            return sensors.AsNoTracking().GetPagedQueryable(
                 filter.PageNum, filter.PageSize);
        }
    }
}
