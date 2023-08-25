using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.SensorTypes
{
    public interface ISensorTypeRepository : IRepository<SensorType>
    {
        IEnumerable<SensorType> GetSensorTypes();
    }
}
