using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Expressions;

namespace BioTonFMS.Telematica;

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
        
        // Собираем трекерные теги из предыдущего сообщения в словарь. 
        var previousTags = previousMessage.Tags
            .Where(t => t.TrackerTagId.HasValue)
            .ToDictionary(previousTag => previousTag.TrackerTagId!.Value);

        // Удаляем из previousTags теги, которые есть в тегах текущего сообщения (для них не нужно FallBackValue)
        foreach (var tag in message.Tags)
        {
            if (!tag.TrackerTagId.HasValue || !previousTags.ContainsKey(tag.TrackerTagId.Value))
                continue;
            previousTags.Remove(tag.TrackerTagId.Value);
        }

        // Добавляет оставшиеся тэги в сообщение
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
    /// <param name="exceptionHandler">Object which executes different parts of formula calculation process.
    /// May be used for exception handling.</param>
    /// <remarks>This static method parses sensor formulas, sorts them by their dependencies and compiles their
    /// ASTs to sorted sequences of runnable lambda expressions. Resulting sequences are paired with ids of their
    /// trackers</remarks>
    /// <returns>One set of compiled sensor formulas per tracker: (tracker1, {(name1, expression1), ...}), (tracker2, {}), ...</returns>
    public static IEnumerable<(int TrackerId, (string Name, Expression? Expression)[])> BuildSensors(this IEnumerable<Tracker> trackers,
        IEnumerable<TrackerTag> trackerTags, IExceptionHandler? exceptionHandler = null)
    {
        var parameters = trackerTags.Where(t =>
                t.DataType is TagDataTypeEnum.Double or TagDataTypeEnum.Integer
                    or TagDataTypeEnum.Byte /* For the moment we can process doubles only */)
            .ToDictionary(t => t.Name, _ => typeof( TagData<double> ) /* For the moment we can process doubles only */);
        return trackers.Select(t => (
            t.Id,
            t.Sensors
                .Select(s => (s.Id.ToString(), (s.Formula, UseFallbacks: s.UseLastReceived)))
                .SortAndBuild(parameters, exceptionHandler)
                .ToArray()));
    }

    /// <summary>
    /// Calculates sensor values using previously compiled sensor expressions
    /// </summary>
    /// <param name="sensorExpressions">Sorted sequence of previously compiled mutually dependent sensor expressions</param>
    /// <param name="messageTags">Set of message tags used as arguments of expressions</param>
    /// <param name="tagNameByIdDict">Dictionary which maps tracker tag ids to respective tag names</param>
    /// <returns>Sequence of calculation results paired with sensor names (for the moment
    /// sensor ids are used as names): (name1, value1), (name2, value2)...</returns>
    public static IEnumerable<(string, object?)> CalculateSensors(this IEnumerable<(string, Expression?)> sensorExpressions,
        IEnumerable<MessageTag> messageTags, IDictionary<int, string> tagNameByIdDict)
    {
        var arguments = messageTags
            .Where(tag =>
                tag is MessageTagDouble or MessageTagInteger or MessageTagByte /* For the moment we can process doubles only */ &&
                tag.TrackerTagId is not null &&
                tagNameByIdDict.ContainsKey(tag.TrackerTagId!.Value))
            .ToDictionary(
                tag => tagNameByIdDict[tag.TrackerTagId!.Value],
                new Func<MessageTag, object?>(tag => new TagData<double>(
                    tag switch
                    {
                        MessageTagByte v => v.Value,
                        MessageTagDouble v => v.Value,
                        MessageTagInteger v => v.Value,
                        _ => throw new Exception("This type of tags is not supported for the moment!")
                    }, tag.IsFallback)));

        var calculatedSensors = sensorExpressions.Execute(arguments);

        return calculatedSensors;
    }

    /// <summary>
    /// Creates message tags by calculated sensor values 
    /// </summary>
    /// <param name="sensorValues">Sequence of calculated sensor values paired with
    /// sensor names (ids): (name1, value1), (name2, value2)...</param>
    /// <param name="message">Message to which those sensor values pertain</param>
    /// <returns>Set of created message tags bound to their sensors and their message
    /// and containing calculated values</returns>
    private static IEnumerable<MessageTag> ConvertSensorValuesToTags(this IEnumerable<(string, object?)> sensorValues,
        TrackerMessage message)
    {
        var messageTags = sensorValues
            .Select(pair => new MessageTagDouble() /* For the moment we can process doubles only */
            {
                TagType = TagDataTypeEnum.Double, SensorId = int.Parse(pair.Item1), TrackerTagId = null, TrackerMessageId = message.Id,
                TrackerMessage = message, IsFallback = false, Value = pair.Item2 as double? ?? double.NaN
            });
        return messageTags;
    }

    /// <summary>
    /// Calculates sensor bound message tags and adds them to their message 
    /// </summary>
    /// <param name="message">Message which contains sensor expression arguments and which will be
    /// modified by adding tags with calculated sensor values</param>
    /// <param name="builtSensors">Previously built sensor expressions</param>
    /// <param name="tagNameByIdDict">Dictionary which maps tracker tag ids to respective tag names</param>
    private static void UpdateSensorTags(this TrackerMessage message, IDictionary<int, (string, Expression?)[]> builtSensors,
        IDictionary<int, string> tagNameByIdDict)
    {
        var builtTrackerSensors = builtSensors[message.TrId];
        var newTags = builtTrackerSensors
            .CalculateSensors(message.Tags, tagNameByIdDict)
            .ConvertSensorValuesToTags(message);
        var newTagList = message.Tags.Where(tag => !tag.SensorId.HasValue).ToList();
        newTagList.AddRange(newTags);
        message.Tags = newTagList;
    }

    /// <summary>
    /// Builds sensor expressions and uses them to calculate sensor values for a set of messages
    /// </summary>
    /// <param name="messages">Messages for which sensor values will be calculates. They will be
    /// modified by adding tags with those sensor values</param>
    /// <param name="trackers">Trackers for messages. Should contain all trackers which are
    /// referenced by the messages</param>
    /// <param name="trackerTags">Tracker tags used to determine names, types and values of sensor parameters</param>
    /// <param name="exceptionHandler">Object which executes different parts of formula calculation process.
    /// May be used for exception handling.</param>
    public static void UpdateSensorTags(this IEnumerable<TrackerMessage> messages, IEnumerable<Tracker> trackers,
        ICollection<TrackerTag> trackerTags, IExceptionHandler? exceptionHandler = null)
    {
        var builtSensors = trackers
            .BuildSensors(trackerTags, exceptionHandler)
            .ToDictionary(t => t.Item1 /* Tracker id */, t => t.Item2);

        var tagNameById = trackerTags
            .Where(tag => tag.DataType is TagDataTypeEnum.Double or TagDataTypeEnum.Byte
                or TagDataTypeEnum.Integer /* For the moment we can process doubles only */)
            .ToDictionary(tag => tag.Id, tag => tag.Name);

        foreach (var message in messages)
        {
            message.UpdateSensorTags(builtSensors, tagNameById);
        }
    }
}
