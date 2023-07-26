using Microsoft.Extensions.Logging;

namespace BiotonFMS.Telematica.Tests.Mocks;

public class LoggerMock<T> : ILogger<T>
{
    public List<string> TraceLogs { get; } = new();
    public List<string> DebugLogs { get; } = new();
    public List<string> InformationLogs { get; } = new();
    public List<string> WarningLogs { get; } = new();
    public List<string> ErrorLogs { get; } = new();
    public List<string> CriticalLogs { get; } = new();
    public List<string> NoneLogs { get; } = new();

    public List<string> GetLogsList(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => TraceLogs,
            LogLevel.Debug => DebugLogs,
            LogLevel.Information => InformationLogs,
            LogLevel.Warning => WarningLogs,
            LogLevel.Error => ErrorLogs,
            LogLevel.Critical => CriticalLogs,
            LogLevel.None => NoneLogs,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };


    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        GetLogsList(logLevel).Add(formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}