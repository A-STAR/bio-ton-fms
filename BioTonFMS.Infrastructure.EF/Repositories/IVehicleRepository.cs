using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTonFMS.Infrastructure.EF.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        public IEnumerable<Device> GetAllDevices(int id);
    }
}
