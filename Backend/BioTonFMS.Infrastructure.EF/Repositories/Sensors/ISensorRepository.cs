using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.Sensors
{
    public interface ISensorRepository : IRepository<Sensor>
    {
        PagedResult<Sensor> GetSensors(SensorsFilter filter);
    }
}
