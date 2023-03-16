using System.Globalization;
using BioTonFMS.Domain;
using BioTonFMS.Expressions;
using BioTonFMS.Telematica.Expressions;
using BiotonFMS.Telematica.Tests.Mocks;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using BioTonFMS.Telematica.Validation;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.IdentityModel.Tokens;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class ValidationTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ValidationTests(ITestOutputHelper testOutputHelper)
    {
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ru");
        _testOutputHelper = testOutputHelper;
    }

    public static IEnumerable<object> SensorSamples => new[]
    {
        new object[]
        {
            "[POSITIVE] Tracker Does Not Exist",
            new Sensor()
            {
                Name = "Sensor", TrackerId = TrackerRepositoryMock.NonExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1, Formula = "const1"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.TrackerId), "*Трекер* не существует*")
            }
        },
        new object[]
        {
            "[POSITIVE] Unit Does Not Exist",
            new Sensor()
            {
                Name = "Sensor", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.NonExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1, Formula = "const1"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.UnitId), "*Единица измерения * не существует*")
            }
        },
        new object[]
        {
            "[POSITIVE] Validator Does Not Exist",
            new Sensor()
            {
                Name = "Sensor", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, ValidatorId = SensorRepositoryMock.NonExistentSensorId,
                ValidationType = ValidationTypeEnum.LogicalAnd, FuelUse = 1, Formula = "const1"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.ValidatorId), "*только на датчики своего трекера*")
            }
        },
        new object[]
        {
            "[POSITIVE] Sensor Type Does Not Exist",
            new Sensor()
            {
                Name = "Sensor", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.NonExistentSensorTypeId, FuelUse = 1, Formula = "const1"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.SensorTypeId), "*Тип датчиков * не существует*")
            }
        },
        new object[]
        {
            "[POSITIVE] Name is too short",
            new Sensor()
            {
                Name = "", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1, Formula = "const1"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Name), "*должно быть длиной*")
            }
        },
        new object[]
        {
            "[POSITIVE] Name is too long",
            new Sensor()
            {
                Name = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901",
                TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1, Formula = "const1"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Name), "*должно быть длиной * символов*")
            }
        },
        new object[]
        {
            "[POSITIVE] Description is too long",
            new Sensor()
            {
                Name = "Sensor", Description = new String('a', 501), TrackerId = TrackerRepositoryMock.ExistentTrackerId,
                UnitId = UnitRepositoryMock.ExistentUnitId, SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1,
                Formula = "const1"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Description), "*должно быть длиной * символов*")
            }
        },
        new object[]
        {
            "[POSITIVE] Formula is too long",
            new Sensor()
            {
                Name = "Sensor", Formula = "const1" + String.Concat(Enumerable.Repeat(" + const1", 100)),
                TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Formula), "*должно быть длиной * символов*")
            }
        },
        new object[]
        {
            "[POSITIVE] Fuel Use is less or equal zero",
            new Sensor()
            {
                Name = "Sensor", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 0, Formula = "const1"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.FuelUse), "*должно быть больше*")
            }
        },
        new object[]
        {
            "[NEGATIVE] Description is empty",
            new Sensor()
            {
                Name = "Sensor", Description = "", TrackerId = TrackerRepositoryMock.ExistentTrackerId,
                UnitId = UnitRepositoryMock.ExistentUnitId, SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1,
                Formula = "const1"
            },
            Array.Empty<SensorProblemDescription>()
        },
        new object[]
        {
            "[NEGATIVE] Description is null",
            new Sensor()
            {
                Name = "Sensor", Description = null!, TrackerId = TrackerRepositoryMock.ExistentTrackerId,
                UnitId = UnitRepositoryMock.ExistentUnitId, SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1,
                Formula = "const1"
            },
            Array.Empty<SensorProblemDescription>()
        },
        new object?[]
        {
            "[NEGATIVE] Ок",
            new Sensor
            {
                Formula = "a", Name = "b", TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1
            },
            Array.Empty<SensorProblemDescription>()
        },
        new object[]
        {
            "[POSITIVE] Пустая формула",
            new Sensor
            {
                Formula = "", Name = "b"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Formula), "Синтаксическая ошибка в выражении!", new Location(0))
            }
        },
        new object[]
        {
            "[POSITIVE] Формула с синтаксической ошибкой",
            new Sensor
            {
                Formula = " s ~", Name = "b"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Formula), "Синтаксическая ошибка в выражении!", new Location(3, 1))
            }
        },
        new object[]
        {
            "[POSITIVE] Ссылка на несуществующий датчик или тэг",
            new Sensor
            {
                Formula = "const1 + snark", Name = "b"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Formula), "Выражение содержит ссылку на несуществующий тэг или датчик!",
                    new Location(9, 5))
            }
        },
        new object[]
        {
            "[POSITIVE] Датчик с таким именем уже существует",
            new Sensor
            {
                Formula = "const1", Name = "a"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Name), "Датчик с именем a уже существует!")
            }
        },
        new object[]
        {
            "[POSITIVE] Тэг с таким именем уже существует",
            new Sensor
            {
                Formula = "const1", Name = "tagA"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Name),
                    "Существует тэг с именем tagA! Имя датчика не должно совпадать с именем тэга")
            }
        },
        new object?[]
        {
            "[NEGATIVE] Сложная формула",
            new Sensor
            {
                Formula = " tagA + a ", Name = "b", TrackerId = TrackerRepositoryMock.ExistentTrackerId,
                UnitId = UnitRepositoryMock.ExistentUnitId, SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1
            },
            Array.Empty<SensorProblemDescription>()
        },
        new object?[]
        {
            "[POSITIVE] Не указан тип валидатора",
            new Sensor
            {
                Formula = "a", Name = "b", ValidatorId = 222
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.ValidationType), "Не указан тип валидатора!")
            }
        },
        new object?[]
        {
            "[POSITIVE] ValidationType не null, хотя ValidatorId null",
            new Sensor
            {
                Formula = "a", Name = "b", ValidationType = ValidationTypeEnum.LogicalAnd
            },
            new[]
            {
                new SensorProblemDescription("warning", "ValidationType не null, хотя ValidatorId null")
            }
        },
        new object?[]
        {
            "[POSITIVE] Ссылка на несуществующий датчик, или датчик другого трекера",
            new Sensor
            {
                Formula = "a", Name = "b", ValidatorId = 2224, ValidationType = ValidationTypeEnum.LogicalAnd
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.ValidatorId), "Валидатор может ссылаться только на датчики своего трекера!")
            }
        },
        new object?[]
        {
            "[POSITIVE] Недопустимый тип валидатора",
            new Sensor
            {
                Formula = "a", Name = "b", ValidatorId = 222, ValidationType = (ValidationTypeEnum)2231
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.ValidationType), "Недопустимый тип валидатора!")
            }
        },
        new object?[]
        {
            "[NEGATIVE] Верный валидатор",
            new Sensor
            {
                Formula = "a", Name = "b", ValidatorId = 222, ValidationType = ValidationTypeEnum.LogicalAnd,
                TrackerId = TrackerRepositoryMock.ExistentTrackerId, UnitId = UnitRepositoryMock.ExistentUnitId,
                SensorTypeId = SensorTypeRepositoryMock.ExistentSensorTypeId, FuelUse = 1
            },
            Array.Empty<SensorProblemDescription>()
        },
    };


    [Theory, MemberData(nameof(SensorSamples))]
    public void ValidateSensor_SimpleValidation(string testName, Sensor sensorToValidate, SensorProblemDescription[] referenceProblems)
    {
        _testOutputHelper.WriteLine(testName);

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
            new SensorValidator(TrackerRepositoryMock.GetStub(), UnitRepositoryMock.GetStub(),
                SensorTypeRepositoryMock.GetStub())).ToArray();

        CheckValidationResults(referenceProblems, validationResults);
    }

    public static IEnumerable<object> SensorSamplesCycles => new[]
    {
        new object?[]
        {
            "[NEGATIVE] Ok",
            new Sensor
            {
                Formula = "const1", Name = "b"
            },
            Array.Empty<SensorProblemDescription>()
        },
        new object[]
        {
            "[POSITIVE] Формула создаёт цикл",
            new Sensor
            {
                Formula = "kebab", Name = "b"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Formula), "Формула создаёт цикл: b -> kebab -> b", new Location(0, 5))
            }
        },
        new object[]
        {
            "[POSITIVE] Ссылки на себя считаются не циклом, а ссылкой на несуществующий",
            new Sensor
            {
                Formula = " b + const1", Name = "b"
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.Formula), "Выражение содержит ссылку на несуществующий тэг или датчик!",
                    new Location(1, 1))
            }
        },
        new object[]
        {
            "[POSITIVE] Валидатор создаёт цикл",
            new Sensor
            {
                Formula = "const1", Name = "b", ValidatorId = 222, ValidationType = ValidationTypeEnum.LogicalAnd
            },
            new[]
            {
                new SensorProblemDescription(nameof(Sensor.ValidatorId), "Валидатор создаёт цикл: b -> kebab -> b")
            }
        },
    };

    [Theory, MemberData(nameof(SensorSamplesCycles))]
    public void ValidateSensor_FindCycles(string testName, Sensor sensorToValidate, SensorProblemDescription[] referenceProblems)
    {
        _testOutputHelper.WriteLine(testName);

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

        var validationResults = Validation.ValidateSensor(tracker, trackerTags, sensorToValidate, LoggerMock.GetStub<int>(),
            FluentValidatorMock.GetStub<Sensor>()).ToArray();
        CheckValidationResults(referenceProblems, validationResults);
    }

    private void CheckValidationResults(SensorProblemDescription[] referenceProblems, SensorProblemDescription[] validationResults)
    {

        if (referenceProblems.IsNullOrEmpty())
        {
            using var scope = new AssertionScope();
            validationResults.Length.Should().Be(0);
            _testOutputHelper.WriteLine("Лишние ошибки валидации:");
            _testOutputHelper.WriteLine("  " + string.Join("\n  ",
                validationResults.Select(r => $"(FieldName: {r.FieldName}, Message: {r.Message}, Location: {r.Location})")));
        }
        else
        {
            validationResults.Length.Should().BeGreaterOrEqualTo(referenceProblems.Length);

            foreach (var spdReference in referenceProblems)
            {
                validationResults.Should().Contain(spd => spd.FieldName == spdReference.FieldName);
                var problemsForField =
                    validationResults.Where(spd => spd.FieldName == spdReference.FieldName).ToArray();
                problemsForField.Select(spd => spd.Message).Should().ContainMatch(spdReference.Message);
                var matchingProblems = problemsForField.Where(spd =>
                {
                    using var scope = new AssertionScope();
                    spd.Message.Should().Match(spdReference.Message);
                    return !scope.Discard().Any();
                });
                matchingProblems.Select(spd => spd.Location).Should().Contain(spdReference.Location);
            }
        }
    }

    [Fact]
    public void ValidateSensorRemoval_NoConflicts_Ok()
    {
        var sensorToDelete = new Sensor()
        {
            Name = "a"
        };

        var tracker = new Tracker
        {
            Id = 111, Sensors = new List<Sensor>
            {
                new()
                {
                    Id = 222, Formula = "b", Name = "kebab"
                },
                sensorToDelete
            }
        };

        var validationResult = Validation.ValidateSensorRemoval(tracker, sensorToDelete, LoggerMock.GetStub<int>());

        validationResult.Should().BeNull();
    }
    
    [Fact]
    public void ValidateSensorRemoval_Conflict_ReturnsErrorMessage()
    {
        var sensorToDelete = new Sensor()
        {
            Name = "a"
        };

        var tracker = new Tracker
        {
            Id = 111, Sensors = new List<Sensor>
            {
                new()
                {
                    Id = 222, Formula = "a", Name = "kebab"
                },
                sensorToDelete
            }
        };

        var validationResult = Validation.ValidateSensorRemoval(tracker, sensorToDelete, LoggerMock.GetStub<int>());

        validationResult.Should().NotBeNull();
        validationResult.Should().Match("*ссылается*");
    }    
}
