using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks.Repositories;

public static class VehicleGroupRepositoryMock
{
    public static IEnumerable<VehicleGroup> GetVehicleGroups() =>
        new List<VehicleGroup>
        {
            new()
            {
                Id = 1,
                Name = "Тракторы"
            },
            new()
            {
                Id = 2,
                Name = "Не тракторы"
            }
        };

    public static IVehicleGroupRepository GetStub()
    {
        var stub = new Mock<IVehicleGroupRepository>();
        stub.Setup(x => x.GetVehicleGroups())
            .Returns(GetVehicleGroups);
        return stub.Object;
    }
}