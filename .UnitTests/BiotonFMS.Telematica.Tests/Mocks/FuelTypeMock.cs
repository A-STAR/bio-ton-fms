using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class FuelTypeMock
{
    public static IEnumerable<FuelType> GetFuelTypes() =>
        new List<FuelType>{
            new()
            {
                Id = 1,
                Name = "Бензин"
            },
            new()
            {
                Id = 2,
                Name = "Дизельное топливо"
            }
        };

    public static IFuelTypeRepository GetStub()
    {
        var stub = new Mock<IFuelTypeRepository>();
        stub.Setup(_ => _.GetFuelTypes()).Returns(GetFuelTypes());
        stub.Setup(x => x[It.IsAny<int>()])
            .Returns((int i) => GetFuelTypes().FirstOrDefault(x => x.Id == i));
        return stub.Object;
    }
}