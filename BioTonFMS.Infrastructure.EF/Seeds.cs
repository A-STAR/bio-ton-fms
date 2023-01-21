using BioTonFMS.Domain;
using Bogus;

namespace BioTonFMS.Infrastructure.EF;

public static class Seeds
{
    public static List<Tracker> GenerateTrackers()
    {
        var sensorId = -1;
        var sensors = new Faker<Sensor>()
            .RuleFor(v => v.Id, (f, v) => sensorId--)
            .RuleFor(v => v.Name, (f, v) => f.Hacker.Noun())
            .RuleFor(v => v.Formula, (f, v) => "someParam");

        var trackerId = -1;
        var trackers = new Faker<Tracker>()
            .RuleFor(v => v.Id, (f, v) => trackerId--)
            .RuleFor(v => v.Name, (f, v) => f.Hacker.Adjective())
            .RuleFor(v => v.Sensors, (f, v) =>
            {
                var result = sensors.Generate(10);
                result.ForEach(s =>
                {
                    s.TrackerId = v.Id;
                });
                return result;
            })
            .RuleFor(v => v.ExternalId, (f, v) => v.Id);

        return trackers.Generate(10);
    }
}
