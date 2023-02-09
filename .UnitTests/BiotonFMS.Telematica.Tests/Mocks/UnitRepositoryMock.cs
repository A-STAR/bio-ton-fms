using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class UnitRepositoryMock
{
    public const int ExistentUnitId = 1;
    public const int NonExistentUnitId = -1;

    public const int MeterUnitId = 1;
    public const int SecondUnitId = 2;

    private static List<Unit> GetUnits() =>
        new()
        {
            new Unit(1, "Meter", "m"), 
            new Unit(2, "Second", "s")
        };

    public static IUnitRepository GetStub()
    {
        var repo = new Mock<IUnitRepository>();
        repo.Setup(x => x.GetUnits())
            .Returns(GetUnits);
        repo.Setup(x => x[It.IsAny<int>()])
            .Returns((int i) => GetUnits().FirstOrDefault(x => x.Id == i));
        return repo.Object;
    }
}
