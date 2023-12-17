
using Xunit.Abstractions;

namespace BiotonFMS.IntegrationTests.Extensions;

internal static class TestOutputHelperExnensions
{
    public static void LogInformation(this ITestOutputHelper testOutputHelper, string message, params object?[] args)
    {
        testOutputHelper.WriteLine($"{DateTime.Now:O} {string.Format(message, args)}");
    }

    public static void LogWarning(this ITestOutputHelper testOutputHelper, string message, params object?[] args)
    {
        testOutputHelper.WriteLine($"{DateTime.Now:O} {string.Format(message, args)}");
    }

    public static void LogDebug(this ITestOutputHelper testOutputHelper, string message, params object?[] args)
    {
        testOutputHelper.WriteLine($"{DateTime.Now:O} {string.Format(message, args)}");
    }

    public static void LogError(this ITestOutputHelper testOutputHelper, Exception exception, string message, params object?[] args)
    {
        testOutputHelper.WriteLine($"{DateTime.Now:O} {string.Format(message, args)}");
        testOutputHelper.WriteLine(exception.StackTrace);
    }

    public static void LogError(this ITestOutputHelper testOutputHelper, string message, params object?[] args)
    {
        testOutputHelper.WriteLine($"{DateTime.Now:O} {string.Format(message, args)}");
    }
}
