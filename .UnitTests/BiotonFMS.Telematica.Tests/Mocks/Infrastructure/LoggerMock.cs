using Microsoft.Extensions.Logging;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure;

public static class LoggerMock
{
    public static ILogger<T> GetStub<T>()
    {
        var mock = new Mock<ILogger<T>>();
        return mock.Object;
    }
}
