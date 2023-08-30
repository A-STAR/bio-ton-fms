using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Migrations.Migrations;
using BioTonFMS.Telematica.Controllers.TestDataController;
using FluentScheduler;

namespace BioTonFMSApp.Scheduler;

public class MoveTestTrackerMessagesJob : IJob
{
    private readonly ILogger<MoveTestTrackerMessagesJob> _logger;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerRepository _trackerRepository;

    public MoveTestTrackerMessagesJob(
        ILogger<MoveTestTrackerMessagesJob> logger,
        ITrackerMessageRepository messageRepository,
        ITrackerRepository trackerRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _trackerRepository = trackerRepository;
    }

    public void Execute()
    {
        var testTrackersExternalIds = _trackerRepository.GetTrackers(new TrackersFilter() { PageSize = 100000}, forUpdate: false)
            .Results.Where(t => t.Id <= 0).Select(t => t.ExternalId).ToArray();

        var testMessages = _messageRepository.GetMessagesForTrackers(testTrackersExternalIds, forUpdate: true);

        // Сдвинуть все тестовые сообщения на "Сегодня"

        foreach (var testMessage in testMessages)
        {
            testMessage.ServerDateTime = ToToday(testMessage.ServerDateTime);
            if (testMessage.TrackerDateTime is not null)
            {
                testMessage.TrackerDateTime = ToToday(testMessage.TrackerDateTime.Value);
            }
            _messageRepository.Update(testMessage);
        }
    }

    private DateTime ToToday(DateTime dateTime)
    {
        var now = DateTime.UtcNow;
        return now.Add(-now.TimeOfDay).Add(dateTime.TimeOfDay).ToUniversalTime();
    }
}
