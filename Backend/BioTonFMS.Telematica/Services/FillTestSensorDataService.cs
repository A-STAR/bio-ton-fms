using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using Microsoft.Extensions.Logging;
using BioTonFMS.Common.Extensions;
using Bogus;

namespace BioTonFMS.Telematica.Services;
public class FillTestSensorDataService
{
    private readonly ILogger<FillTestSensorDataService> _logger;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerRepository _trackerRepository;

    public FillTestSensorDataService(
        ILogger<FillTestSensorDataService> logger,
        ITrackerMessageRepository messageRepository,
        ITrackerRepository trackerRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _trackerRepository = trackerRepository;
    }

    public void FillTestSensorData()
    {
        var testTrackersExternalIds = _trackerRepository.GetTrackers(new TrackersFilter() { PageSize = 100000 }, forUpdate: false)
            .Results.Where(t => t.Id <= 0).Select(t => t.ExternalId).ToArray();

        var testTrackersDict = _trackerRepository.GetTrackers(new TrackersFilter() { PageSize = 100000 }, forUpdate: false)
            .Results.ToDictionary(t => t.ExternalId); 
        
        var testMessages = _messageRepository.GetMessagesForTrackers(testTrackersExternalIds, forUpdate: true);

        // Для каждого сообщения сгенерить теги для значений датчиков, если их ещё не было
        foreach (var testMessage in testMessages)
        {
            // если у сообщения нет тегов датчика, то добавляем случайные 
            if (testMessage.Tags.All(t => t.SensorId != null))
            {
                var tracker = testTrackersDict[testMessage.ExternalTrackerId];
                if (tracker != null)
                {
                    var sensors = tracker.Sensors;

                    var facker = new Faker();
                    for (int i = 0; i < facker.Random.Int(0, sensors.Count()); i++) 
                    { 
                        var sensor = sensors.ToArray()[i];
                        if (sensor != null)
                        {

                        }

                    }

                }
            }

            _messageRepository.Update(testMessage);
        }
    }
}
