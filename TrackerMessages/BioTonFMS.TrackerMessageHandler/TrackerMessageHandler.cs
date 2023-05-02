using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.MessageProcessing;
using BioTonFMS.TrackerMessageHandler.MessageParsing;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;

namespace BioTonFMS.TrackerMessageHandler;

public class TrackerMessageHandler : IBusMessageHandler
{
    private readonly ILogger<TrackerMessageHandler> _logger;
    private readonly Func<TrackerTypeEnum, IMessageParser> _parserProvider;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerTagRepository _trackerTagRepository;
    private readonly ITrackerRepository _trackerRepository;

    public TrackerMessageHandler(ILogger<TrackerMessageHandler> logger,
        Func<TrackerTypeEnum, IMessageParser> parserProvider, ITrackerMessageRepository messageRepository,
        ITrackerTagRepository trackerTagRepository, ITrackerRepository trackerRepository)
    {
        _logger = logger;
        _parserProvider = parserProvider;
        _messageRepository = messageRepository;
        _trackerTagRepository = trackerTagRepository;
        _trackerRepository = trackerRepository;
    }

    public Task HandleAsync(byte[] binaryMessage)
    {
        var messageText = Encoding.UTF8.GetString(binaryMessage);
        _logger.LogDebug("Получен пакет {MessageText}", messageText);

        var rawMessage = JsonSerializer.Deserialize<RawTrackerMessage>(messageText)
            ?? throw new ArgumentException("Невозможно разобрать сырое сообщение", nameof(binaryMessage));

        if (_messageRepository.ExistsByUid(rawMessage.PackageUID)) return Task.CompletedTask;

        TrackerMessage[] messages = _parserProvider(rawMessage.TrackerType).ParseMessage(rawMessage.RawMessage, rawMessage.PackageUID)
            .ToArray();

        var trackers = new HashSet<Tracker>();
        foreach (var trackerMessage in messages)
        {
            var tracker = _trackerRepository.FindTracker(trackerMessage.Imei, trackerMessage.ExternalTrackerId);
            if (tracker is null) continue;
            trackers.Add(tracker);
            
            tracker.SetTrackerAddress(rawMessage.IpAddress, rawMessage.Port);
            _trackerRepository.Update(tracker);
            
            trackerMessage.ExternalTrackerId = tracker.ExternalId;
        }

        try
        {
            CalculateSensorTags(trackers, messages);
        }
        catch( Exception ex )
        {
            _logger.LogError(ex, "Ошибка при вычислении тегов датчиков");
        }

        foreach (var trackerMessage in messages)
        {
            _messageRepository.Add(trackerMessage);
            _logger.LogDebug("Добавлено новое сообщение из пакета {PackageUID} Id сообщения {MessageId} TrId={TrId} Imei={Imei}",
                rawMessage.PackageUID, trackerMessage.Id, trackerMessage.ExternalTrackerId, trackerMessage.Imei);
        }

        return Task.CompletedTask;
    }
    
    private void CalculateSensorTags(ISet<Tracker> trackers, TrackerMessage[] messages)
    {
        if (trackers.Count == 0 || messages.Length == 0) return;

        var externalTrackerIds = trackers.Where(t => t.ExternalId != 0).Select(t => t.ExternalId).ToArray();
        IDictionary<int, TrackerMessage> previousMessages = _messageRepository.GetLastMessagesFor(externalTrackerIds);
        
        TrackerTag[] trackerTags = _trackerTagRepository.GetTags().ToArray();

        messages.UpdateSensorTags(previousMessages, trackers, trackerTags, new LoggingExceptionHandler(_logger));
    }
}
