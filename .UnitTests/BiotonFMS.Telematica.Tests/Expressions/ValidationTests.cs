using BioTonFMS.Domain;
using BioTonFMS.Expressions;
using BioTonFMS.Telematica.Expressions;
using BiotonFMS.Telematica.Tests.Mocks;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class ValidationTests
{
    public static IEnumerable<object> SensorSamples => new[]
    {
        new object?[]
        {
            new Sensor
            {
                Formula = "a", Name = "b"
            },
            null
        },
        new object[]
        {
            new Sensor
            {
                Formula = "", Name = "b"
            },
            new SensorProblemDescription(nameof(Sensor.Formula), "Синтаксическая ошибка в выражении!", new Location(0))
        },
        new object[]
        {
            new Sensor
            {
                Formula = " s 1", Name = "b"
            },
            new SensorProblemDescription(nameof(Sensor.Formula), "Синтаксическая ошибка в выражении!", new Location(3, 1))
        },
        new object[]
        {
            new Sensor
            {
                Formula = "const1 + snark", Name = "b"
            },
            new SensorProblemDescription(nameof(Sensor.Formula), "Выражение содержит ссылку на несуществующий тэг или датчик!",
                new Location(9, 5))
        },
        new object[]
        {
            new Sensor
            {
                Formula = "const1", Name = "a"
            },
            new SensorProblemDescription(nameof(Sensor.Name), "Датчик с именем a уже существует!")
        },
        new object[]
        {
            new Sensor
            {
                Formula = "const1", Name = "tagA"
            },
            new SensorProblemDescription(nameof(Sensor.Name), "Существует тэг с именем tagA! Имя датчика не должно совпадать с именем тэга")
        },
        new object?[]
        {
            new Sensor
            {
                Formula = " tagA + a ", Name = "b"
            },
            null
        },
        new object?[]
        {
            new Sensor
            {
                Formula = "a", Name = "b", ValidatorId = 222
            },
            new SensorProblemDescription(nameof(Sensor.ValidationType), "Не указан тип валидатора!")
        },
        new object?[]
        {
            new Sensor
            {
                Formula = "a", Name = "b", ValidationType = ValidationTypeEnum.LogicalAnd
            },
            new SensorProblemDescription("warning", "ValidationType не null, хотя ValidatorId null")
        },
        new object?[]
        {
            new Sensor
            {
                Formula = "a", Name = "b", ValidatorId = 2224, ValidationType = ValidationTypeEnum.LogicalAnd
            },
            new SensorProblemDescription(nameof(Sensor.ValidatorId), "Валидатор может ссылаться только на датчики своего трекера!")
        },
        new object?[]
        {
            new Sensor
            {
                Formula = "a", Name = "b", ValidatorId = 222, ValidationType = (ValidationTypeEnum)2231
            },
            new SensorProblemDescription(nameof(Sensor.ValidationType), "Недопустимый тип валидатора!")
        },
        new object?[]
        {
            new Sensor
            {
                Formula = "a", Name = "b", ValidatorId = 222, ValidationType = ValidationTypeEnum.LogicalAnd
            },
            null
        },
    };


    [Theory, MemberData(nameof(SensorSamples))]
    public void ValidateSensor_SimpleValidation(Sensor sensorToValidate, SensorProblemDescription? referenceProblemDescription)
    {
        var tracker = new Tracker
        {
            Id = 111, Sensors = new List<Sensor>
            {
                new()
                {
                    Id = 222, Formula = "1", Name = "a"
                },
                sensorToValidate
            }
        };

        var trackerTags = new TrackerTag[]
        {
            new()
            {
                Name = "tagA", DataType = TagDataTypeEnum.Double
            }
        };

        var validationResults = Validation.ValidateSensor(tracker, trackerTags, sensorToValidate, LoggerMock.GetStub<int>(),
            FluentValidatorMock.GetStub<Sensor>()).ToArray();

        if (referenceProblemDescription == null)
        {
            validationResults.Length.Should().Be(0);
        }
        else
        {
            validationResults.Length.Should().Be(1);
            validationResults[0].Should().BeEquivalentTo(referenceProblemDescription);
        }
    }

    public static IEnumerable<object> SensorSamplesCycles => new[]
    {
        new object?[]
        {
            new Sensor
            {
                Formula = "const1", Name = "b"
            },
            null
        },
        new object[]
        {
            new Sensor
            {
                Formula = "kebab", Name = "b"
            },
            new SensorProblemDescription(nameof(Sensor.Formula), "Формула создаёт цикл: b -> kebab -> b", new Location(0, 5))
        },
        new object[]
        {
            new Sensor
            {
                Formula = " b + const1", Name = "b"
            },
            new SensorProblemDescription(nameof(Sensor.Formula), "Выражение содержит ссылку на несуществующий тэг или датчик!",
                new Location(1, 1))
        },
        new object[]
        {
            new Sensor
            {
                Formula = "const1", Name = "b", ValidatorId = 222, ValidationType = ValidationTypeEnum.LogicalAnd
            },
            new SensorProblemDescription(nameof(Sensor.ValidatorId), "Валидатор создаёт цикл: b -> kebab -> b")
        },
    };

    [Theory, MemberData(nameof(SensorSamplesCycles))]
    public void ValidateSensor_FindCycles(Sensor sensorToValidate, SensorProblemDescription? referenceProblemDescription)
    {
        var tracker = new Tracker
        {
            Id = 111, Sensors = new List<Sensor>
            {
                new()
                {
                    Id = 222, Formula = "b", Name = "kebab"
                },
                sensorToValidate
            }
        };

        var trackerTags = new TrackerTag[]
        {
            new()
            {
                Name = "tagA", DataType = TagDataTypeEnum.Double
            }
        };

        var validationResults = Validation.ValidateSensor(tracker, trackerTags, sensorToValidate, LoggerMock.GetStub<int>(), FluentValidatorMock.GetStub<Sensor>()).ToArray();

        if (referenceProblemDescription == null)
        {
            validationResults.Length.Should().Be(0);
        }
        else
        {
            validationResults.Length.Should().Be(1);
            validationResults[0].Should().BeEquivalentTo(referenceProblemDescription);
        }
    }
}
