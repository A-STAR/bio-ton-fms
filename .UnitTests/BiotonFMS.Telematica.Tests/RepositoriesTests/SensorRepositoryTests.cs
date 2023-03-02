using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.RepositoriesTests;

public class SensorRepositoryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SensorRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #region Filters
    public static IEnumerable<object[]> FilterTestData =>
        new[]
        {
            new object[]
            {
                "No Filtering",
                new SensorsFilter(),
                new[]
                {
                    1, 2, 3, 4
                }
            },
            new object[]
            {
                "Pagination Filter",
                new SensorsFilter
                {
                    PageNum = 1, PageSize = 2
                },
                new[]
                {
                    1, 2
                }
            },
            new object[]
            {
                "Tracker Filter 1",
                new SensorsFilter
                {
                    TrackerId = 1
                },
                new[]
                {
                    1, 2, 4
                }
            },
            new object[]
            {
                "Tracker Filter 2",
                new SensorsFilter
                {
                    TrackerId = 2
                },
                new[]
                {
                    3
                }
            },
            new object[]
            {
                "Sorting Ascending",
                new SensorsFilter
                {
                    SortBy = SensorSortBy.Name
                },
                new[]
                {
                    2, 3, 4, 1
                }
            },
            new object[]
            {
                "Sorting Descending",
                new SensorsFilter
                {
                    SortBy = SensorSortBy.Name, SortDirection = SortDirection.Descending
                },
                new[]
                {
                    1, 4, 3, 2
                }
            }
        };

    [Theory, MemberData(nameof(FilterTestData))]
    public void GetSensors_WithFilters_ShouldFilter(string testName, SensorsFilter filter, int[] expected)
    {
        _testOutputHelper.WriteLine(testName);

        var results = CreateSensorRepository(SampleSensors).GetSensors(filter).Results;

        results.Select(s => s.Id).Should().Equal(expected);
    }
    #endregion

    #region Constraints for Add
    public static IEnumerable<object[]> ConstraintViolationTestData =>
        new[]
        {
            new object[]
            {
                "[POSITIVE] Duplicate Name Inside Tracker 1",
                new Sensor()
                {
                    Id = -1, TrackerId = 1, Name = "Sensor 2"
                },
                "*Датчик с именем * уже существует*"
            },
            new object[]
            {
                "[POSITIVE] Duplicate Name Inside Tracker 2",
                new Sensor()
                {
                    Id = -1, TrackerId = 2, Name = "Sensor 3"
                },
                "*Датчик с именем * уже существует*"
            },
            new object[]
            {
                "[NEGATIVE] Globally Duplicate Name",
                new Sensor()
                {
                    Id = -1, TrackerId = 1, Name = "Sensor 3"
                },
                ""
            },
        };

    [Theory, MemberData(nameof(ConstraintViolationTestData))]
    public void AddSensors_SensorWithConstraintViolation_ThrowsArgumentException(string testName, Sensor sensor, string expectedMessage)
    {
        _testOutputHelper.WriteLine(testName);

        var actionAssertions = CreateSensorRepository(SampleSensors).Invoking(r => r.Add(sensor)).Should();
        if (expectedMessage.IsNullOrEmpty())
            actionAssertions.NotThrow();
        else
            actionAssertions.Throw<ArgumentException>().WithMessage(expectedMessage);
    }
    #endregion

    [Fact]
    public void Remove_SensorReferencedAsValidatorByOtherSensor_ThrowsArgumentException()
    {
        var sensorRepository = CreateSensorRepository(SampleSensors);
        sensorRepository.Invoking(r => r.Remove(new Sensor()
        {
            Id = 2
        })).Should().Throw<ArgumentException>("*удалить датчик*используется*валидатор*");
        sensorRepository[2].Should().NotBeNull();
    }
    
    [Fact]
    public void Remove_SensorNotReferencedAsValidatorByOtherSensor_DoesNotThrow()
    {
        var sensorRepository = CreateSensorRepository(SampleSensors);
        sensorRepository.Invoking(r => r.Remove(new Sensor()
        {
            Id = 3
        })).Should().NotThrow();
        sensorRepository[3].Should().BeNull();
    }
    

    private static SensorRepository CreateSensorRepository(ICollection<Sensor> sensorList)
    {
        IKeyValueProvider<Sensor, int> keyValueProviderMock = new KeyValueProviderMock<Sensor, int>(sensorList);
        IQueryableProvider<Sensor> sensorQueryProviderMock = new QueryableProviderMock<Sensor>(sensorList);
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactoryMock = new BioTonDBContextUnitOfWorkFactoryMock();

        var repo = new SensorRepository(LoggerMock.GetSensorRepositoryStub(), keyValueProviderMock, sensorQueryProviderMock,
            unitOfWorkFactoryMock);
        return repo;
    }

    private static IList<Sensor> SampleSensors => new List<Sensor>
    {
        new()
        {
            Id = 1, Name = "XSensor 1", TrackerId = 1, Formula = "\"Sensor 2\" + \"Sensor 4\"",
            Description = "Description of Sensor 1", ValidatorId = 2
        },
        new()
        {
            Id = 2, Name = "Sensor 2", TrackerId = 1, Formula = "a",
            Description = "Description of Sensor 2",
        },
        new()
        {
            Id = 4, Name = "Sensor 4", TrackerId = 1, Formula = "b",
            Description = "Description of Sensor 4",
        },
        new()
        {
            Id = 3, Name = "Sensor 3", TrackerId = 2, Formula = "c",
            Description = "Description of Sensor 3",
        },
    };
}
