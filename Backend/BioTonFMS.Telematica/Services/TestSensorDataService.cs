using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using Microsoft.Extensions.Logging;
using BioTonFMS.Common.Extensions;
using Bogus;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;

namespace BioTonFMS.Telematica.Services;
public class TestSensorDataService
{
    private readonly ILogger<TestSensorDataService> _logger;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerRepository _trackerRepository;

    public TestSensorDataService(
        ILogger<TestSensorDataService> logger,
        ITrackerMessageRepository messageRepository,
        ITrackerRepository trackerRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _trackerRepository = trackerRepository;
    }

    public void FillRandomSensorData()
    {
        var testTrackersExternalIds = _trackerRepository.GetTrackers(new TrackersFilter() { PageSize = 100000 }, forUpdate: false)
            .Results.Where(t => t.Id <= 0).Select(t => t.ExternalId).ToArray();

        var testTrackersDict = _trackerRepository.GetTrackers(new TrackersFilter() { PageSize = 100000 }, forUpdate: false)
            .Results.ToDictionary(t => t.ExternalId); 
        
        var testMessages = _messageRepository.GetMessagesForTrackers(testTrackersExternalIds, forUpdate: true);

        // Для каждого сообщения сгенерить теги для значений датчиков, если их ещё не было
        var faker = new Faker();
        foreach (var testMessage in testMessages)
        {
            // если у сообщения нет тегов датчика, то добавляем случайные 
            if (testMessage.Tags.All(t => t.SensorId == null))
            {
                var tracker = testTrackersDict[testMessage.ExternalTrackerId];
                if (tracker != null)
                {
                    var sensors = tracker.Sensors;
                    for (int i = 0; i < sensors.Count(); i++) 
                    { 
                        var sensor = sensors.ToArray()[i];
                        if (sensor != null)
                        {
                            switch (sensor.DataType)
                            {
                                case SensorDataTypeEnum.Boolean:
                                    var tagBool = new MessageTagBoolean()
                                    {
                                        TagType = TagDataTypeEnum.Boolean,
                                        SensorId = sensor.Id,
                                        TrackerTagId = null,
                                        TrackerMessageId = testMessage.Id,
                                        TrackerMessage = testMessage,
                                        IsFallback = false,
                                        Value = faker.Random.Bool()
                                    };
                                    testMessage.Tags.Add(tagBool);
                                    break;
                                case SensorDataTypeEnum.Number:
                                    var tagNum = new MessageTagDouble()
                                    {
                                        TagType = TagDataTypeEnum.Double,
                                        SensorId = sensor.Id,
                                        TrackerTagId = null,
                                        TrackerMessageId = testMessage.Id,
                                        TrackerMessage = testMessage,
                                        IsFallback = false,
                                        Value = faker.Random.Double()
                                    };
                                    testMessage.Tags.Add(tagNum);
                                    break;
                                default: break;
                            }
                        }
                    }
                }
            }

            _messageRepository.Update(testMessage);
        }
    }
}
