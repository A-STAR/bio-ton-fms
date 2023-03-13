using System.Collections;
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
            .RuleFor(v => v.Formula,
                (f, _) => filteredTrackerTags[f.Random.Int(0, filteredTrackerTags.Length - 1)].Name)
            .RuleFor(v => v.SensorTypeId, (f, _) => sensorTypeIds[f.Random.Int(0, sensorTypeIds.Length - 1)])
            .RuleFor(v => v.UnitId, (f, _) => unitIds[f.Random.Int(0, unitIds.Length - 1)]);

        var tagId = -1;
        var messageId = -1;
        var message = new Faker<TrackerMessage>()
            .RuleFor(v => v.Id, () => messageId--)
            .RuleFor(v => v.Tags, (f, v) => GenerateTags(f, v, trackerTags, ref tagId))
            .RuleFor(v => v.Latitude, (f, _) => f.Random.Double(-90, 90).OrNull(f, .1f))
            .RuleFor(v => v.Longitude, (f, v) => v.Latitude is null ? null : f.Random.Double(-180, 180))
            .RuleFor(v => v.Speed, (f, _) => f.Random.Double(0, 120).OrNull(f, .1f))
            .RuleFor(v => v.Height, (f, _) => f.Random.Double(-90, 90).OrNull(f, .1f))
            .RuleFor(v => v.CoolantTemperature, (f, _) => f.Random.Int(-40, 180).OrNull(f, .1f))
            .RuleFor(v => v.Direction, (f, _) => f.Random.Double(-180, 180).OrNull(f, .1f))
            .RuleFor(v => v.FuelLevel, (f, _) => f.Random.Int(0, 100).OrNull(f, .1f))
            .RuleFor(v => v.EngineSpeed, (f, _) => f.Random.Int(0, 10000).OrNull(f, .1f))
            .RuleFor(v => v.SatNumber, (f, _) => f.Random.Int(0, 10).OrNull(f, .1f))
            .RuleFor(v => v.CoordCorrectness, (f, _) => f.Random.Enum<CoordCorrectnessEnum>().OrNull(f, .1f))
            .RuleFor(v => v.TrackerDateTime, (f, _) => f.Time())
            .RuleFor(v => v.ServerDateTime, (f, _) => f.Time())
            .RuleFor(v => v.PackageUID, Guid.NewGuid);

        var trackerId = -1;
        var tracker = new Faker<Tracker>()
            .RuleFor(v => v.Id, (_, _) => trackerId--)
            .RuleFor(v => v.Name, (f, _) => f.Hacker.Adjective())
            .RuleFor(v => v.Sensors, (_, v) =>
            {
                var result = sensor.Generate(10);
                result.ForEach(s => { s.TrackerId = v.Id; });
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
                    s.Imei = v.Tracker.Imei;
                });
                return result.ToArray();
            });

        return trackerData.Generate(10);
    }

    private static MessageTag[] GenerateTags(Faker f, TrackerMessage v, TrackerTag[] t, ref int id)
    {
        var result = new MessageTag[f.Random.Int(0, 12)];
        var possibleTypes = t.Select(x => x.DataType)
            .Distinct()
            .Where(x => x != TagDataTypeEnum.Struct)
            .ToArray();
        var exclude = Enum.GetValues<TagDataTypeEnum>()
            .Where(x => !possibleTypes.Contains(x))
            .ToArray();
        
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = f.Random.Enum(exclude)
                switch
                {
                    TagDataTypeEnum.Integer => new MessageTagInteger
                    {
                        Value = f.Random.Int(),
                        TagType = TagDataTypeEnum.Integer,
                        TrackerTagId = f.Random
                            .ArrayElement(t.Where(x => x.DataType == TagDataTypeEnum.Integer).ToArray())
                            .Id
                    },
                    TagDataTypeEnum.Bits => new MessageTagBits
                    {
                        Value = new BitArray(f.Random.Bytes(2)),
                        TagType = TagDataTypeEnum.Bits,
                        TrackerTagId = f.Random
                            .ArrayElement(t.Where(x => x.DataType == TagDataTypeEnum.Bits).ToArray())
                            .Id
                    },
                    TagDataTypeEnum.Byte => new MessageTagByte
                    {
                        Value = f.Random.Byte(),
                        TagType = TagDataTypeEnum.Byte,
                        TrackerTagId = f.Random
                            .ArrayElement(t.Where(x => x.DataType == TagDataTypeEnum.Byte).ToArray())
                            .Id
                    },
                    TagDataTypeEnum.Double => new MessageTagDouble
                    {
                        Value = f.Random.Double(),
                        TagType = TagDataTypeEnum.Double,
                        TrackerTagId = f.Random
                            .ArrayElement(t.Where(x => x.DataType == TagDataTypeEnum.Double).ToArray())
                            .Id
                    },
                    TagDataTypeEnum.Boolean => new MessageTagBoolean
                    {
                        Value = f.Random.Bool(),
                        TagType = TagDataTypeEnum.Boolean,
                        TrackerTagId = f.Random
                            .ArrayElement(t.Where(x => x.DataType == TagDataTypeEnum.Boolean).ToArray())
                            .Id
                    },
                    TagDataTypeEnum.String => new MessageTagString
                    {
                        Value = f.Hacker.Abbreviation(),
                        TagType = TagDataTypeEnum.String,
                        TrackerTagId = f.Random
                            .ArrayElement(t.Where(x => x.DataType == TagDataTypeEnum.String).ToArray())
                            .Id
                    },
                    TagDataTypeEnum.DateTime => new MessageTagDateTime
                    {
                        Value = f.Time(),
                        TagType = TagDataTypeEnum.DateTime,
                        TrackerTagId = f.Random
                            .ArrayElement(t.Where(x => x.DataType == TagDataTypeEnum.DateTime).ToArray())
                            .Id
                    },
                    TagDataTypeEnum.Struct => throw new ArgumentOutOfRangeException(),
                    _ => throw new ArgumentOutOfRangeException()
                };

            result[i].Id = id--;
            result[i].TrackerMessageId = v.Id;
            result[i].TrackerMessage = v;
            result[i].SensorId = null;
            result[i].IsFallback = false;
        }

        return result;
    }

    private static DateTime Time(this Faker f) =>
        DateTime.UtcNow;
}