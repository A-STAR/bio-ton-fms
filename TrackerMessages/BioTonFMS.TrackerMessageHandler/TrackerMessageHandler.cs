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

        
        Tracker? tracker = null;
        if (messages.Length > 0)
        {
            var firstMessge = messages[0];
            tracker = _trackerRepository.FindTracker(firstMessge.Imei, firstMessge.ExternalTrackerId);
        }
        
        if (tracker is not null)
        {
            foreach (var trackerMessage in messages)
            {
                tracker.SetTrackerAddress(rawMessage.IpAddress, rawMessage.Port);
                _trackerRepository.Update(tracker);

                trackerMessage.ExternalTrackerId = tracker.ExternalId;
            }

            try
            {
                CalculateSensorTags(tracker, messages);
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "Ошибка при вычислении тегов датчиков");
            }
        }
        else
        {
            _logger.LogWarning("Пропущен расчет датчиков из-за того, что не удалось найти трекер");
        }

        foreach (var trackerMessage in messages)
        {
            _messageRepository.Add(trackerMessage);
            _logger.LogDebug("Добавлено новое сообщение из пакета {PackageUID} Id сообщения {MessageId} TrId={TrId} Imei={Imei}",
                rawMessage.PackageUID, trackerMessage.Id, trackerMessage.ExternalTrackerId, trackerMessage.Imei);
        }

        return Task.CompletedTask;
    }

    private void CalculateSensorTags(Tracker tracker, TrackerMessage[] messages)
    {
        TrackerMessage? previousMessage = _messageRepository.GetLastMessageFor(tracker.ExternalId);

        TrackerTag[] trackerTags = _trackerTagRepository.GetTags().ToArray();

        if (previousMessage is not null)
        {
            messages.UpdateSensorTags(new Dictionary<int, TrackerMessage>() { { previousMessage.ExternalTrackerId, previousMessage } },
                new[] { tracker }, trackerTags, new LoggingExceptionHandler(_logger));
        }
    }
}
