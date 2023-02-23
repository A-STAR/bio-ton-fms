using System.Text.RegularExpressions;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using Bogus;
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace BioTonFMS.Telematica.Controllers.TestData;

public class TrackerData
{
    public Tracker? Tracker;
    public TrackerMessage[]? Messages;
}
public static class Seeds
{
    public static List<TrackerData> GenerateTrackers(int[] sensorTypeIds, int[] unitIds, TrackerTag[] trackerTags)
    {
        var filteredTrackerTags = trackerTags
            .Where(t => t.DataType is TagDataTypeEnum.Integer or TagDataTypeEnum.Double or TagDataTypeEnum.Byte
                && Regex.IsMatch(t.Name, "^[a-zA-Z_][a-zA-Z_0-9]*$")).ToArray();

        var sensorId = -1;
        var sensor = new Faker<Sensor>()
            .RuleFor(v => v.Id, (_, _) => sensorId--)
            .RuleFor(v => v.Name, (f, _) => f.Hacker.Noun() + -sensorId)
            .RuleFor(v => v.Formula, (f, _) => filteredTrackerTags[f.Random.Int(0, filteredTrackerTags.Length - 1)].Name)
            .RuleFor(v => v.SensorTypeId, (f, _) => sensorTypeIds[f.Random.Int(0, sensorTypeIds.Length - 1)])
            .RuleFor(v => v.UnitId, (f, _) => unitIds[f.Random.Int(0, unitIds.Length - 1)]);

        var tagId = -1;

        var messageId = -1;
        var message = new Faker<TrackerMessage>()
            .RuleFor(v => v.Id, () => messageId--)
            .RuleFor(v => v.Tags, (f, v) => trackerTags.Select<TrackerTag, MessageTag>(t => new MessageTagDouble()
            {
                Id = tagId--, TrackerMessageId = v.Id, TrackerMessage = v, TagType = 4,
                TrackerTagId = t.Id, SensorId = null, Value = f.Random.Double(-100, 100), IsFallback = false,
            }).ToList());

        var trackerId = -1;
        var tracker = new Faker<Tracker>()
            .RuleFor(v => v.Id, (_, _) => trackerId--)
            .RuleFor(v => v.Name, (f, _) => f.Hacker.Adjective())
            .RuleFor(v => v.Sensors, (_, v) =>
            {
                var result = sensor.Generate(10);
                result.ForEach(s =>
                {
                    s.TrackerId = v.Id;
                });
                return result;
            })
            .RuleFor(v => v.ExternalId, (_, v) => v.Id)
            .RuleFor(v => v.TrackerType, (f, _) => f.Random.Enum<TrackerTypeEnum>())
            .RuleFor(v => v.SimNumber, (f, _) => f.Random.ReplaceNumbers("############"))
            .RuleFor(v => v.Imei, (f, _) => f.Random.ReplaceNumbers("###############"));


        var trackerData = new Faker<TrackerData>()
            .RuleFor(v => v.Tracker, () => tracker)
            .RuleFor(v => v.Messages, (_, v) =>
            {
                var result = message.Generate(10);
                result.ForEach(s =>
                {
                    s.TrId = v.Tracker!.Id;
                });
                return result.ToArray();
            });

        return trackerData.Generate(10);
    }
}
