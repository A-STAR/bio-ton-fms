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
            new object[]
            {
                2552,
                DateTime.UnixEpoch.AddHours(-100),
                DateTime.UnixEpoch.AddHours(100),
                new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 3,
                    TotalTime = 20
                }
            },
            new object[]
            {
                1555,
                DateTime.UnixEpoch.AddHours(-100),
                DateTime.UnixEpoch.AddHours(100),
                new ViewMessageStatisticsDto
                {
                    NumberOfMessages = 0
                }
            },
            new object[]
            {
                -12938,
                DateTime.UnixEpoch.AddHours(-100),
                DateTime.UnixEpoch.AddHours(100),
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