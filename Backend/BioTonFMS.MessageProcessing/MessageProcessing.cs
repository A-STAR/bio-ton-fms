using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.Compilation;
using Serilog;

namespace BioTonFMS.MessageProcessing;

public static class MessageProcessing
{
    /// <summary>
    /// Calculates fall back tag values for message
    /// </summary>
    /// <param name="message">Message for which fall back tag values are calculated. It is
    /// modified by adding new tags to it</param>
    /// <param name="previousMessage">Message which serves as a source of fall back values</param>
    public static void CalculateFallBackValues(TrackerMessage message, TrackerMessage? previousMessage)
    {
        if (previousMessage is null)
            return;

        // Puts tracker tags from the previous message into dictionary
        var previousTags = previousMessage.Tags
            .Where(t => t.TrackerTagId.HasValue)
            .ToDictionary(previousTag => previousTag.TrackerTagId!.Value);

        // Delete tags which exist in current message from previous because they do not need fallbacks
        foreach (var tag in message.Tags)
        {
            if (!tag.TrackerTagId.HasValue || !previousTags.ContainsKey(tag.TrackerTagId.Value))
                continue;
            previousTags.Remove(tag.TrackerTagId.Value);
        }

        // Adds remaining tags to the current message
        foreach (var previousTag in previousTags)
        {
            if (previousTag.Value is not MessageTagDouble doubleTag)
                continue;
            message.Tags.Add(new MessageTagDouble()
            {
                TrackerMessageId = message.Id, TrackerMessage = message, TrackerTagId = doubleTag.TrackerTagId, TagType = doubleTag.TagType,
                Value = doubleTag.Value, IsFallback = true
            });
        }
    }

    /// <summary>
    /// Builds sensor expressions for supplied set of trackers
    /// </summary>
    /// <param name="trackers">Trackers for which sensor expressions are built</param>
    /// <param name="trackerTags">Set of tracker tags which are used as parameters for sensor formulas</param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    /// <remarks>This static method parses sensor formulas, sorts them by their dependencies and compiles their
    /// ASTs to sorted sequences of runnable lambda expressions. Resulting sequences are paired with ids of their
    /// trackers</remarks>
    /// <returns>A sequence of compiled sensor formulas per tracker: (externalTrackerId1, {(name1, expression1), ...}), (externalTrackerId2, {}), ...</returns>
    public static IEnumerable<(int ExternalTrackerId, CompiledExpression<SensorExpressionProperties>[] SensorsCompiledExpressions)> BuildSensors(
        this IEnumerable<Tracker> trackers,
        IEnumerable<TrackerTag> trackerTags, IExceptionHandler? exceptionHandler = null)
    {
        // строит из набора тегов словарь <название тега, тип тега>
        Dictionary<string, Type> parameterTypesDictionary = BuildParameterDictionaryByTags(trackerTags);

        var compilationOptions = new CompilationOptions()
        {
            ExpressionBuilderFactory = new SensorExpressionBuilderFactory()
        };

        return trackers.Select(t =>
        {
            var sensorNameById = t.Sensors.ToDictionary(s => s.Id, s => s.Name);
            return (
                t.ExternalId,
                t.Sensors
                    .Select(s => new SensorExpressionProperties(s, s.ValidatorId == null ? null : sensorNameById[s.ValidatorId.Value]))
                    .SortAndBuild(parameterTypesDictionary, Postprocess.AddSensorValidatorToAst, compilationOptions, exceptionHandler)
                    .ToArray());
        });
    }

    /// <summary>
    /// Builds parameter dictionary by tags
    /// </summary>
    /// <param name="trackerTags">Tags</param>
    /// <returns>Parameter dictionary (name -> type)</returns>
    private static Dictionary<string, Type> BuildParameterDictionaryByTags(IEnumerable<TrackerTag> trackerTags)
    {

        var parameters = trackerTags.Where(t =>
                t.DataType is TagDataTypeEnum.Double or TagDataTypeEnum.Integer
                    or TagDataTypeEnum.Byte /* For the moment we can process numbers only */)
            .ToDictionary(t => t.Name, _ => typeof( TagData<double> ) /* For the moment we can process doubles only */);
        return parameters;
    }

    /// <summary>
    /// Calculates sensor values using previously compiled sensor expressions
    /// </summary>
    /// <param name="sensorExpressions">Sorted sequence of previously compiled mutually dependent sensor expressions</param>
    /// <param name="messageTags">Set of message tags used as arguments of expressions</param>
    /// <param name="previousMessageTags"></param>
    /// <param name="tagNameByIdDict">Dictionary which maps tracker tag ids to respective tag names</param>
    /// <returns>Sequence of calculation results paired with sensor names (for the moment
    /// sensor ids are used as names): (name1, value1), (name2, value2)...</returns>
    public static IEnumerable<(SensorExpressionProperties, object?)> CalculateSensors(
        this IEnumerable<CompiledExpression<SensorExpressionProperties>> sensorExpressions, IEnumerable<MessageTag> messageTags,
        IEnumerable<MessageTag>? previousMessageTags, IDictionary<int, string> tagNameByIdDict)
    {
        // Add arguments for current message
        Dictionary<string, object?> argumentsByNames = messageTags
            .CalcArguments(tagNameByIdDict)
            .ToDictionary(p => p.Key, p => p.Value);

        // Add arguments for previous message
        if (previousMessageTags is not null)
            foreach (var pair in previousMessageTags.CalcArguments(tagNameByIdDict))
                argumentsByNames.Add("#" + pair.Key, pair.Value);

        // Calculate sensors
        var calculatedSensors = sensorExpressions.Execute(argumentsByNames);

        return calculatedSensors;
    }

    /// <summary>
    /// Builds arguments dictionary by message tags
    /// </summary>
    /// <param name="messageTags">Message tags from updated message</param>
    /// <param name="tagNameByIdDict">Dictionary which maps tag ids to tag names (id -> name)</param>
    /// <returns>Pairs of argument names and argument values</returns>
    private static IEnumerable<KeyValuePair<string, object?>> CalcArguments(this IEnumerable<MessageTag> messageTags,
        IDictionary<int, string> tagNameByIdDict)
    {

        var arguments = messageTags
            .Where(tag =>
                tag is MessageTagDouble or MessageTagInteger or MessageTagByte /* For the moment we can process doubles only */ &&
                tag.TrackerTagId is not null &&
                tagNameByIdDict.ContainsKey(tag.TrackerTagId!.Value))
            .Select(
                tag => new KeyValuePair<string, object?>(
                    tagNameByIdDict[tag.TrackerTagId!.Value],
                    new TagData<double>(
                        tag switch
                        {
                            MessageTagByte v => v.Value,
                            MessageTagDouble v => v.Value,
                            MessageTagInteger v => v.Value,
                            _ => throw new Exception("This type of tags is not supported for the moment!")
                        }, tag.IsFallback)));
        return arguments;
    }

    /// <summary>
    /// Creates message tags by calculated sensor values 
    /// </summary>
    /// <param name="sensorValues">Sequence of calculated sensor values paired with
    /// sensor names (ids): (name1, value1), (name2, value2)...</param>
    /// <param name="message">Message to which those sensor values pertain</param>
    /// <returns>Set of created message tags bound to their sensors and their message
    /// and containing calculated values</returns>
    private static IEnumerable<MessageTag> ConvertSensorValuesToTags(
        this IEnumerable<(SensorExpressionProperties Props, object? Value)> sensorValues, TrackerMessage message)
    {
        var messageTags = sensorValues
            .Select(pair => new MessageTagDouble() /* For the moment we can process doubles only */
            {
                TagType = TagDataTypeEnum.Double, SensorId = pair.Props.Sensor.Id, TrackerTagId = null, TrackerMessageId = message.Id,
                TrackerMessage = message, IsFallback = false, Value = (pair.Item2 as TagData<double>?)?.Value ?? double.NaN
            });
        return messageTags;
    }

    /// <summary>
    /// Calculates sensor bound message tags and adds them to their message 
    /// </summary>
    /// <param name="message">Message which contains sensor expression arguments and which will be
    ///     modified by removing old sensor tags and then adding sensor tags with new sensor values</param>
    /// <param name="previousMessage"></param>
    /// <param name="builtSensorsByExternalTrackerId">Dictionary of previously built sensor expressions by external tracker ID</param>
    /// <param name="tagNameByIdDict">Dictionary which maps tracker tag ids to respective tag names</param>
    private static void UpdateSensorTags(this TrackerMessage message, TrackerMessage? previousMessage,
        IDictionary<int, CompiledExpression<SensorExpressionProperties>[]> builtSensorsByExternalTrackerId,
        IDictionary<int, string> tagNameByIdDict)
    {
        // Get sequence of built sensors for tracker 
        if (!builtSensorsByExternalTrackerId.TryGetValue(message.ExternalTrackerId, out var builtTrackerSensors))
            return;

        // Calculate sensor values and put the values to message tags
        IEnumerable<MessageTag> newTags = builtTrackerSensors
            .CalculateSensors(message.Tags, previousMessage?.Tags, tagNameByIdDict)
            .ConvertSensorValuesToTags(message);
        if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
        {
            var newTagsList = newTags.Select(t => $"Id={t.Id} SensorId={t.SensorId} Value={t.ValueString} TrackerMessageId={t.TrackerMessageId}");
            Log.Verbose("Теги для датчиков {@newTags}", newTagsList);
        }
        
        // Create new tag list then fill it with existing message tags and
        // add message tags for sensors 
        var newListOfTags = message.Tags.Where(tag => !tag.SensorId.HasValue).ToList();
        newListOfTags.AddRange(newTags);

        // Set new list instead of old list
        message.Tags = newListOfTags;
    }

    /// <summary>
    /// Builds sensor expressions and uses them to calculate sensor values for a set of messages
    /// </summary>
    /// <param name="messages">Sequence of messages for which sensor values will be calculated. They will be
    /// modified by removing old sensor tags and then adding tags with new sensor values</param>
    /// <param name="previousMessages">Сообщения предшествующие последовательности обрабатываемых сообщений
    /// для каждого из трекеров</param>
    /// <param name="trackers">Trackers for messages. Should contain all trackers which are
    /// referenced by the messages</param>
    /// <param name="trackerTags">Tracker tags used to determine names, types and values of sensor parameters</param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    public static void UpdateSensorTags(this IEnumerable<TrackerMessage> messages, IDictionary<int, TrackerMessage> previousMessages,
        IEnumerable<Tracker> trackers,
        ICollection<TrackerTag> trackerTags, IExceptionHandler? exceptionHandler = null)
    {
        // Построить по набору выражений датчиков для каждого из трекеров
        Dictionary<int, CompiledExpression<SensorExpressionProperties>[]> builtSensorsByExternalTrackerId = trackers
            .BuildSensors(trackerTags, exceptionHandler)
            .ToDictionary(t => t.ExternalTrackerId, t => t.SensorsCompiledExpressions);

        // Build dictionary which maps tag ids to tag names
        Dictionary<int, string> tagNameByIdDict = trackerTags
            .Where(tag => tag.DataType is TagDataTypeEnum.Double or TagDataTypeEnum.Byte
                or TagDataTypeEnum.Integer /* For the moment we can process doubles only */)
            .ToDictionary(tag => tag.Id, tag => tag.Name);

        // Update sensor tags for all messages in sequence
        foreach (var message in messages)
        {
            previousMessages.TryGetValue(message.ExternalTrackerId, out var previousMessage);
            Log.Verbose("Перед UpdateSensorTags для сообщения {MessageId}", message.Id);
            message.UpdateSensorTags(previousMessage, builtSensorsByExternalTrackerId, tagNameByIdDict);
            previousMessages[message.ExternalTrackerId] = message;
        }
    }
}
