using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Paging;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class VehicleMock
{
    public static PagedResult<Vehicle> GetVehicles(VehiclesFilter filter) =>
        new()
        {
            CurrentPage = 1,
            Results = new List<Vehicle>
            {
                new()
                {
                    Id = 1,
                    Name = "Синий Трактор",
                    Make = "Трактор",
                    Model = "Синий",
                    FuelTypeId = 1,
                    FuelType = new FuelType
                    {
                        Id = 1,
                        Name = "Этил"
                    },
                    ManufacturingYear = 1991,
                    Type = VehicleTypeEnum.Transport,
                    VehicleSubType = VehicleSubTypeEnum.Tractor
                },
                new()
                {
                    Id = 2,
                    Name = "Красный Трактор",
                    Make = "Трактор",
                    Model = "Красный",
                    FuelTypeId = 1,
                    FuelType = new FuelType
                    {
                        Id = 1,
                        Name = "Этил"
                    },
                    ManufacturingYear = 1995,
                    Type = VehicleTypeEnum.Transport,
                    VehicleSubType = VehicleSubTypeEnum.Tractor
                }
            }
        };

    public static IVehicleRepository GetStub()
    {
        var stub = new Mock<IVehicleRepository>();
        stub.Setup(x => x.GetVehicles(It.IsAny<VehiclesFilter>()))
            .Returns(GetVehicles);
        stub.Setup(x => x[It.IsAny<int>()])
            .Returns((int i) => GetVehicles(new VehiclesFilter()).Results.FirstOrDefault(x => x.Id == i));
        return stub.Object;
    }
}