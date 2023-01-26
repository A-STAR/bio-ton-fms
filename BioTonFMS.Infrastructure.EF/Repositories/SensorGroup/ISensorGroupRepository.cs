using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.SensorGroups
{
    public interface ISensorGroupRepository : IRepository<SensorGroup>
    {
        IEnumerable<SensorGroup> GetSensorGroups();
    }
}
