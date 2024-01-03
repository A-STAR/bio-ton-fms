using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using Microsoft.Extensions.Logging;
using BioTonFMS.Common.Extensions;
using Bogus;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.MessageProcessing;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Common.Testable;
using System.Data;

namespace BioTonFMS.Telematica.Services;
public class TestCommandsDataService
{
    private readonly ILogger<TestCommandsDataService> _logger;
    private readonly ITrackerCommandRepository _commandRepository;
    private readonly ITrackerRepository _trackerRepository;

    public TestCommandsDataService(
        ILogger<TestCommandsDataService> logger,
        ITrackerCommandRepository commandRepository,
        ITrackerRepository trackerRepository)
    {
        _logger = logger;
        _commandRepository = commandRepository;
        _trackerRepository = trackerRepository;
    }

    public void FillTestCommands()
    {
        var testTrackersDict = _trackerRepository.GetTrackers(new TrackersFilter() { PageSize = 100000 }, forUpdate: false)
            .Results.ToDictionary(t => t.ExternalId);

        var todayStart = DateTime.Today.ToUniversalTime();
        var todayEnd = DateTime.Today.AddDays(1).ToUniversalTime();
        var faker = new Faker();
        var randomCommands = new (string Command, string Response)[]
        {
            ("IMEI", "IMEI 868345032073546"),
            ("IMEI22", "Unrecognized:IMEI22"),
            ("GSMinfo", "SIM0,897010270378428417ff,25002,868345032073546"),
            ("status", "Dev1734 Soft=27.5 Pack=124424 TmDt=17:42:38 21.07.23 Nav=0 Lat=51.582291 Lon=45.976421 Alt=133 Spd=0.0 HDOP=0.5 SatCnt=21 A=96.3 Commit=BC01D9BE"),
            ("status", "Dev1708 Soft=27.5 Pack=451936 TmDt=06:20:12 20.08.23 Nav=0 Lat=53.263488 Lon=50.418068 Alt=78 Spd=0.0 HDOP=0.5 SatCnt=20 A=0.0 Commit=BC01D9BE "),
            ("INFO", "Unrecognized:INFO")
        };

        foreach (var tracker in testTrackersDict.Values)
        {
            var todaysCommands = _commandRepository.GetCommandMessages(tracker.ExternalId, todayStart, todayEnd, 1, 100);
            if (todaysCommands.TotalRowCount <= 0)
            {
                for (int i = 0; i < faker.Random.Int(0, 60); i++)
                {
                    var randomCommand = faker.Random.ArrayElement(randomCommands);
                    DateTime sentTime = todayStart.AddMinutes(faker.Random.Int(1, 23 * 60));
                    DateTime? responseTime = null;
                    if (faker.Random.Int(0, 9) < 7)
                    {
                        responseTime = sentTime.AddSeconds(faker.Random.Int(10, 3 * 60));
                    }
                    var command = new TrackerCommand()
                    {
                        TrackerId = tracker.Id,
                        SentDateTime = sentTime,
                        CommandText = randomCommand.Command,
                        ResponseDateTime = responseTime,
                        ResponseText = responseTime == null? "" : randomCommand.Response
                    };
                    _commandRepository.Add(command);
                }
            }
        }
    }

    public void MoveTestCommandsForToday(DateOnly? fromDate = null)
    {
        var testTrackerIds = _trackerRepository.GetTrackers(new TrackersFilter() { PageSize = 100000 }, forUpdate: false)
            .Results.Where(t => t.Id <= 0).Select(t => t.Id).ToArray();

        if (fromDate is null)
        {
            // сообщения за предыдущий день
            fromDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        }

        IList<TrackerCommand> testCommands = _commandRepository.GetCommandsForDate(testTrackerIds, fromDate.Value, forUpdate: true);

        // Сдвинуть все тестовые команды на "Сегодня"
        foreach (var command in testCommands)
        {
            command.SentDateTime = command.SentDateTime.ToToday();
            if (command.ResponseDateTime is not null)
            {
                command.ResponseDateTime = command.ResponseDateTime.Value.ToToday();
            }
            _commandRepository.Update(command);
        }
    }

}
