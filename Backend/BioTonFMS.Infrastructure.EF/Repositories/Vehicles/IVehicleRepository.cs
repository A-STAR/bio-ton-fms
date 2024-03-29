﻿using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.Vehicles
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        PagedResult<Vehicle> GetVehicles(VehiclesFilter filter, bool hydrate = true);
        Vehicle[] FindVehicles(string? findCriterion);
        IDictionary<int, int> GetExternalIds(params int[] vehicleIds);
        IDictionary<int, string> GetNamesWhereTrackerNotEmpty(params int[] vehicleIds);
        Tracker? GetTracker(int vehicleId);
    }
}
