using BioTonFMS.Common.Testable;
using BioTonFMS.Domain.MessagesView;
using BiotonFMS.Telematica.Tests.Mocks.Repositories;
using FluentAssertions;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.RepositoriesTests;

public class TrackerCommandRepositoryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TrackerCommandRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #region GetStatistics

    public static IEnumerable<object[]> StatisticsData =>
        new List<object[]>
        {
            // Трекер существует, есть сообщения в периоде
            new object[]
            {
                2552,
                SystemTime.UtcNow.AddHours(-100),
                SystemTime.UtcNow.AddHours(100),
                new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 3,
                    TotalTime = 20
                }
            },
            // Трекер существует, нет сообщений в периоде
            new object[]
            {
                1555,
                SystemTime.UtcNow.AddHours(-100),
                SystemTime.UtcNow.AddHours(100),
                new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 0
                }
            },
            // Трекер не существует
            new object[]
            {
                -12938,
                SystemTime.UtcNow.AddHours(-100),
                SystemTime.UtcNow.AddHours(100),
                new ViewMessageStatisticsDto()
            },
        };
    
    [Theory, MemberData(nameof(StatisticsData))]
    public void GetStatistics(int externalId, DateTime start, DateTime end,
        ViewMessageStatisticsDto expected)
    {
        _testOutputHelper.WriteLine($"id: {externalId}; dates: {start} - {end}");

        var result = TrackerCommandRepositoryMock.GetStub().GetStatistics(externalId, start, end);

        expected.Should().BeEquivalentTo(result);
    }

    #endregion
}