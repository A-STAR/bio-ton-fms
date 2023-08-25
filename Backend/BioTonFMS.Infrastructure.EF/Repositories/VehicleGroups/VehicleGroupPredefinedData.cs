using BioTonFMS.Domain;

namespace BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;

public static class VehicleGroupPredefinedData
{
    public static readonly IEnumerable<VehicleGroup> VehicleGroups = new[]
    {
        new VehicleGroup()
        {
            Id = 1,
            Name = "Группа 1"
        },
        new VehicleGroup()
        {
            Id = 2,
            Name = "Группа 2"
        }
    };
}

