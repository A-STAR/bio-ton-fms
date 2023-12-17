using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace BiotonFMS.IntegrationTests.Utils.Mocks;

public class TestOutputHelperLoggerMock<T> : ILogger<T>
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TestOutputHelperLoggerMock(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _testOutputHelper.WriteLine($"{DateTime.Now:O} {formatter(state, exception)}");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}