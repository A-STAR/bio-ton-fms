using System.Diagnostics;
using System.Text.RegularExpressions;
using BioTonFMS.Domain;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.Ast;
using BioTonFMS.Expressions.Compilation;
using BioTonFMS.Expressions.Parsing;
using BioTonFMS.Expressions.Util;
using BioTonFMS.MessageProcessing;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Expressions;

public static class ValidationExtension
{
    public static IEnumerable<SensorProblemDescription> ValidateSensor(this Tracker tracker, IEnumerable<TrackerTag> trackerTags, Sensor sensor,
        ILogger logger, IValidator<Sensor> sensorValidator)
    {
        Debug.Assert(tracker.Sensors.Contains(sensor));

        var validationResults = sensorValidator.Validate(sensor);
        var problemList = validationResults.Errors
            .Select(e => new SensorProblemDescription(e.PropertyName, e.ErrorMessage)).ToList();

        if (!new Regex("^[a-zA-Z_]+[a-zA-Z_0-9]*$").IsMatch(sensor.Name))
        {
            problemList.Add(new SensorProblemDescription(nameof(sensor.Name), $"Имя датчика не соответствует требованиям!"));
        }

        if (tracker.Sensors.Exists(s => s.Name == sensor.Name && !ReferenceEquals(s, sensor)))
        {
            problemList.Add(new SensorProblemDescription(nameof(sensor.Name), $"Датчик с именем {sensor.Name} уже существует"));
        }

        var existingSensorsByName = tracker.Sensors.Where(s => !ReferenceEquals(s, sensor)).ToDictionary(s => s.Name);
        var tagsByName = trackerTags.ToDictionary(t => t.Name);

        if (tagsByName.ContainsKey(sensor.Name))
        {
            problemList.Add(new SensorProblemDescription(nameof(sensor.Name),
                $"Существует параметр трекера с именем {sensor.Name}. Имя датчика не должно совпадать с именем параметра трекера"));
        }

        AstNode? ast = null;
        try
        {
            ast = Parser.Parse(sensor.Formula);
        }
        catch( ParsingException e )
        {
            problemList.Add(new SensorProblemDescription(nameof(sensor.Formula), "Синтаксическая ошибка в выражении", e.Location));
        }

        Variable[]? parameters = null;
        if (ast != null)
        {
            parameters = ast.GetVariables().ToArray();
            foreach (var parameter in parameters)
            {
                if (!existingSensorsByName.ContainsKey(parameter.Name) && !tagsByName.ContainsKey(parameter.Name))
                {
                    problemList.Add(new SensorProblemDescription(nameof(sensor.Formula),
                        "Выражение содержит ссылку на несуществующий параметр трекера или датчик", parameter.Location));
                }
            }
        }

        if (sensor.ValidatorId != null)
        {
            if (!sensor.ValidationType.HasValue)
            {
                problemList.Add(new SensorProblemDescription(nameof(sensor.ValidationType),
                    "Не указан тип валидатора"));
            }
            else if (!Enum.IsDefined(typeof( ValidationTypeEnum ), sensor.ValidationType))
            {
                problemList.Add(new SensorProblemDescription(nameof(sensor.ValidationType), "Недопустимый тип валидатора"));
            }

            // Проверяем, что "валидатор" ссылается на датчик внутри трекера
            if (!tracker.Sensors.Exists(s => s.Id == sensor.ValidatorId))
            {
                problemList.Add(new SensorProblemDescription(nameof(sensor.ValidatorId),
                    "Валидатор может ссылаться только на датчики своего трекера"));
            }
        }
        else
        {
            if (sensor.ValidationType.HasValue)
            {
                problemList.Add(new SensorProblemDescription("warning", "ValidationType не null, хотя ValidatorId null"));
            }
            sensor.ValidationType = null;
        }

        // Дальнейшая валидация требует соблюдения базовых ограничений
        if (problemList.Count != 0)
            return problemList;

        var sensorById = tracker.Sensors.ToDictionary(s => s.Id);

        var expressions = tracker.Sensors
            .Select(s => new SensorExpressionProperties(s, s.ValidatorId.HasValue ? sensorById[s.ValidatorId.Value].Name : null));
        var graph = Helpers.BuildExpressionGraph(expressions, new LoggingExceptionHandler(logger), Postprocess.PostprocessAst);

        var cyclePath = graph.FindAnyLoop(sensor.Name);

        if (cyclePath.Count != 0)
        {
            if (cyclePath.Count == 1)
            {
                problemList.Add(new SensorProblemDescription("error",
                    "Внутренняя ошибка. Путь цикла не должен содержать менее двух узлов"));
                return problemList;
            }
            if (sensor.ValidatorId.HasValue && cyclePath[1] == sensorById[sensor.ValidatorId.Value].Name)
            {
                problemList.Add(new SensorProblemDescription(nameof(Sensor.ValidatorId),
                    $"Валидатор создаёт цикл: {string.Join(" -> ", cyclePath)} -> {sensor.Name}"));
            }
            if (parameters != null)
            {
                var conflictingParameters = parameters.Where(p => p.Name == cyclePath[1]).ToArray();
                if (conflictingParameters.Length != 0)
                {
                    problemList.Add(new SensorProblemDescription(nameof(Sensor.Formula),
                        $"Формула создаёт цикл: {string.Join(" -> ", cyclePath)} -> {sensor.Name}", conflictingParameters[0].Location));
                }
            }
        }

        return problemList;
    }

    public static string? ValidateSensorRemoval(this Tracker tracker, Sensor sensorToDelete, ILogger logger)
    {
        var sensorById = tracker.Sensors.ToDictionary(s => s.Id);
        var expressions = tracker.Sensors
            .Select(s => new SensorExpressionProperties(s, s.ValidatorId.HasValue ? sensorById[s.ValidatorId.Value].Name : null));
        var graph = Helpers.BuildExpressionGraph(expressions, new LoggingExceptionHandler(logger), Postprocess.PostprocessAst);

        var referencingNodes = graph.Where(node => node.Value.Edges.Contains(sensorToDelete.Name)).ToArray();
        return referencingNodes.Any() ? $"На удаляемый датчик ссылается датчик с именем {referencingNodes[0].Key}" : null;
    }
}
