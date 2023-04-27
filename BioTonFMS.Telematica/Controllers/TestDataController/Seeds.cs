using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using Bogus;
using System.Collections;
using System.Text.RegularExpressions;

// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace BioTonFMS.Telematica.Controllers.TestDataController;

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
            .RuleFor(v => v.Name, (f, _) => Regex.Replace(f.Hacker.Noun(), "\\s", "_") + -sensorId)
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
            .RuleFor(v => v.Altitude, (f, _) => f.Random.Double(-90, 90).OrNull(f, .1f))
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
                    s.ExternalTrackerId = v.Tracker!.Id;
                    s.Imei = v.Tracker.Imei;
                });
                return result.ToArray();
            });

        return trackerData.Generate(10);
    }

    public static List<Vehicle> GenerateVehicles(Tracker[] trackers, int[] vehicleGroupIds, int[] fuelTypeIds)
    {
        var vehicleId = -1;
        var vehicleFaker = new Faker<Vehicle>()
            .RuleFor(v => v.Id, (_, _) => vehicleId--)
            .RuleFor(v => v.Name, (f, _) => f.Hacker.Adjective() + -(vehicleId+1))
            .RuleFor(v => v.Type, (f, _) => f.Random.Enum<VehicleTypeEnum>())
            .RuleFor(v => v.VehicleGroupId, (f, _) => vehicleGroupIds.Length == 0 ? null : vehicleGroupIds[f.Random.Int(0, vehicleGroupIds.Length - 1)])
            .RuleFor(v => v.Make, (f, _) => f.Vehicle.Manufacturer())
            .RuleFor(v => v.Model, (f, _) => f.Vehicle.Model())
            .RuleFor(v => v.VehicleSubType, (f, _) => f.Random.Enum<VehicleSubTypeEnum>())
            .RuleFor(v => v.FuelTypeId, (f, _) => fuelTypeIds[f.Random.Int(0, fuelTypeIds.Length - 1)])
            .RuleFor(v => v.ManufacturingYear, (f, _) => f.Random.Int(2000, 2023).OrNull(f, .1f))
            .RuleFor(v => v.RegistrationNumber, (f, _) => f.Random.Replace("?###?? ###"))
            .RuleFor(v => v.TrackerId, (f, vv) =>
            {
                if (trackers.Length == 0)
                    return null;
                var tracker = trackers[f.Random.Int(0, trackers.Length - 1)];
                trackers = trackers.Where(t => t != tracker).ToArray();
                return tracker.Id;
            }
            );

        return vehicleFaker.Generate(10);
    }

    private static MessageTag[] GenerateTags(Faker f, TrackerMessage v, TrackerTag[] t, ref int id)
    {
        var result = new MessageTag[f.Random.Int(0, 12)];
        var tags = f.Random.ArrayElements(t.Where(x => x.DataType != TagDataTypeEnum.Struct).ToArray(), result.Length);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = tags[i].DataType
                switch
            {
                TagDataTypeEnum.Integer => new MessageTagInteger
                {
                    Value = f.Random.Int(-100, 1000),
                    TagType = TagDataTypeEnum.Integer
                },
                TagDataTypeEnum.Bits => new MessageTagBits
                {
                    Value = new BitArray(f.Random.Bytes(2)),
                    TagType = TagDataTypeEnum.Bits
                },
                TagDataTypeEnum.Byte => new MessageTagByte
                {
                    Value = f.Random.Byte(),
                    TagType = TagDataTypeEnum.Byte
                },
                TagDataTypeEnum.Double => new MessageTagDouble
                {
                    Value = f.Random.Double(-100, 1000),
                    TagType = TagDataTypeEnum.Double
                },
                TagDataTypeEnum.Boolean => new MessageTagBoolean
                {
                    Value = f.Random.Bool(),
                    TagType = TagDataTypeEnum.Boolean
                },
                TagDataTypeEnum.String => new MessageTagString
                {
                    Value = f.Hacker.Abbreviation(),
                    TagType = TagDataTypeEnum.String
                },
                TagDataTypeEnum.DateTime => new MessageTagDateTime
                {
                    Value = f.Time(),
                    TagType = TagDataTypeEnum.DateTime
                },
                TagDataTypeEnum.Struct => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };

            result[i].Id = id--;
            result[i].TrackerMessageId = v.Id;
            result[i].TrackerMessage = v;
            result[i].SensorId = null;
            result[i].IsFallback = false;
            result[i].TrackerTagId = tags[i].Id;
        }

        return result;
    }

    private static DateTime Time(this Faker f) =>
        DateTime.UtcNow;
}