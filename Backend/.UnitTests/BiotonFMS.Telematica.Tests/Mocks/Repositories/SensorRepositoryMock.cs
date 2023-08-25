using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using BioTonFMS.Infrastructure.Paging;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks.Repositories;

public static class SensorRepositoryMock
{
    public const int ExistentSensorId = 1;
    public const int NonExistentSensorId = -1;
    public const int ReferencedSensorId = 2;

    private static PagedResult<Sensor> GetSensorsPaged() =>
        new()
        {
            CurrentPage = 1, Results = GetSensors().ToList()
        };

    public static IEnumerable<Sensor> GetSensors() => new List<Sensor>
    {
        new()
        {
            Id = 1, TrackerId = 1, Name = "a", Formula = "b"
        },
        new()
        {
            Id = 2, TrackerId = 1, Name = "b", Formula = "const1"
        }
    };

    public static Sensor? LastUpdateArgument;
    public static Sensor? LastAddArgument;

    public static ISensorRepository GetStub()
    {
        var repo = new Mock<ISensorRepository>();
        repo.Setup(x => x.GetSensors(It.IsAny<SensorsFilter>()))
            .Returns(GetSensorsPaged);
        repo.Setup(x => x[It.IsAny<int>()])
            .Returns((int i) => GetSensorsPaged().Results.FirstOrDefault(x => x.Id == i));
        repo.Setup(x => x.Update(It.IsAny<Sensor>())).Callback((Sensor x) => LastUpdateArgument = x);
        repo.Setup(x => x.Add(It.IsAny<Sensor>())).Callback((Sensor x) => LastAddArgument = x);
        return repo.Object;
    }
}
