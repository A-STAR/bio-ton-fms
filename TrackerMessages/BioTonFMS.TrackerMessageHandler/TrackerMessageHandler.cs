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

        var deserialized = JsonSerializer.Deserialize<RawTrackerMessage>(messageText)
            ?? throw new ArgumentException("Невозможно разобрать сырое сообщение", nameof(binaryMessage));

        if (_messageRepository.ExistsByUid(deserialized.PackageUID)) return Task.CompletedTask;

        TrackerMessage[] messages = _parserProvider(deserialized.TrackerType).ParseMessage(deserialized.RawMessage, deserialized.PackageUID)
            .ToArray();

        CalculateSensorTags(messages);

        foreach (var trackerMessage in messages)
        {
            _messageRepository.Add(trackerMessage);
            _logger.LogDebug("Добавлено новое сообщение из пакета {PackageUID} Id сообщения {MessageId} TrId={TrId} Imei={Imei}",
                deserialized.PackageUID, trackerMessage.Id, trackerMessage.ExternalTrackerId, trackerMessage.Imei);
        }

        return Task.CompletedTask;
    }
    private void CalculateSensorTags(TrackerMessage[] messages)
    {
        if (messages.Length <= 0) return;;

        TrackerMessage oneOfMessages = messages[0];
        if (string.IsNullOrEmpty(oneOfMessages.Imei) && oneOfMessages.ExternalTrackerId == 0) return;

        TrackerMessage? previousMessage = _messageRepository.GetLastMessageFor(oneOfMessages);
        TrackerTag[] trackerTags = _trackerTagRepository.GetTags().ToArray();

        var trackerFilter = oneOfMessages.ExternalTrackerId == 0
            ? new TrackersFilter
            {
                Imei = oneOfMessages.Imei
            }
            : new TrackersFilter
            {
                ExternalId = oneOfMessages.ExternalTrackerId
            };

        var trackers = _trackerRepository.GetTrackers(trackerFilter);

        if (trackers.Results.Count != 1) return;

        messages.UpdateSensorTags(previousMessage, new[]
            {
                trackers.Results[0]
            },
            trackerTags, new LoggingExceptionHandler(_logger));
    }
}
