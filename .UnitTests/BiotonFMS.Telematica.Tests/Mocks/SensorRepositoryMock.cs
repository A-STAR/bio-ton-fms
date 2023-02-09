using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using BioTonFMS.Infrastructure.Paging;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class SensorRepositoryMock
{
    public const int ExistentSensorId = 1;
    public const int NonExistentSensorId = -1;

    private static PagedResult<Sensor> GetSensors() =>
        new()
        {
            CurrentPage = 1,
            Results = new List<Sensor>
            {
                new()
                {
                    Id = 1
                },
                new()
                {
                    Id = 2
                }
            }
        };

    public static Sensor? LastUpdateArgument = null;
    public static Sensor? LastAddArgument = null;

    public static ISensorRepository GetStub()
    {
        var repo = new Mock<ISensorRepository>();
        repo.Setup(x => x.GetSensors(It.IsAny<SensorsFilter>()))
            .Returns(GetSensors);
        repo.Setup(x => x[It.IsAny<int>()])
            .Returns((int i) => GetSensors().Results.FirstOrDefault(x => x.Id == i));
        repo.Setup(x => x.Update(It.IsAny<Sensor>())).Callback((Sensor x) => LastUpdateArgument = x);
        repo.Setup(x => x.Add(It.IsAny<Sensor>())).Callback((Sensor x) => LastAddArgument = x);
        return repo.Object;
    }
}