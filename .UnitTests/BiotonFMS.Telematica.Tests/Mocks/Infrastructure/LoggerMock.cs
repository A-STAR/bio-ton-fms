using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using Microsoft.Extensions.Logging;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure;

public static class LoggerMock
{
    public static ILogger<SensorRepository> GetSensorRepositoryStub()
    {
        var mock = new Mock<ILogger<SensorRepository>>();
        return mock.Object;
    }
}
