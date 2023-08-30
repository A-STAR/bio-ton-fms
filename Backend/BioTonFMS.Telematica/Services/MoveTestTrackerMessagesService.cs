using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using Microsoft.Extensions.Logging;
using BioTonFMS.Common.Extensions;

namespace BioTonFMS.Telematica.Services;
public class MoveTestTrackerMessagesService
{
    private readonly ILogger<MoveTestTrackerMessagesService> _logger;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerRepository _trackerRepository;

    public MoveTestTrackerMessagesService(
        ILogger<MoveTestTrackerMessagesService> logger,
        ITrackerMessageRepository messageRepository,
        ITrackerRepository trackerRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _trackerRepository = trackerRepository;
    }

    public void MoveTestTrackerMessagesForToday(DateOnly? fromDate = null)
    {
        var testTrackersExternalIds = _trackerRepository.GetTrackers(new TrackersFilter() { PageSize = 100000 }, forUpdate: false)
            .Results.Where(t => t.Id <= 0).Select(t => t.ExternalId).ToArray();
        
        if (fromDate is null)
        {
            // сообщения за предыдущий день
            fromDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        }
        var testMessages = _messageRepository.GetTrackerMessagesForDate(testTrackersExternalIds, fromDate.Value, forUpdate: true);

        // Сдвинуть все тестовые сообщения на "Сегодня"
        foreach (var testMessage in testMessages)
        {
            testMessage.ServerDateTime = testMessage.ServerDateTime.ToToday();
            if (testMessage.TrackerDateTime is not null)
            {
                testMessage.TrackerDateTime = testMessage.TrackerDateTime.Value.ToToday();
            }
            _messageRepository.Update(testMessage);
        }
    }
}
