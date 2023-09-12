using System.Text;
using System.Text.Json;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.MessageProcessing;
using BioTonFMS.TrackerMessageHandler.Retranslation;
using BioTonFMS.TrackerProtocolSpecific.CommandCodecs;
using BioTonFMS.TrackerProtocolSpecific.TrackerMessages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioTonFMS.TrackerMessageHandler.Handlers;

public class TrackerMessageHandler : IBusMessageHandler
{
    private readonly ILogger<TrackerMessageHandler> _logger;
    private readonly Func<TrackerTypeEnum, ITrackerMessageParser> _parserProvider;
    private readonly Func<TrackerTypeEnum, ICommandCodec> _commandCodecProvider;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerTagRepository _trackerTagRepository;
    private readonly ITrackerRepository _trackerRepository;
    private readonly IMessageBus _retranslatorBus;
    private readonly IMessageBus _commandReceiveBus;
    private readonly RetranslatorOptions _retranslatorOptions;
    private readonly IMessageBus _consumerBus;

    public TrackerMessageHandler(ILogger<TrackerMessageHandler> logger,
        Func<TrackerTypeEnum, ITrackerMessageParser> parserProvider,
        Func<TrackerTypeEnum, ICommandCodec> codecProvider,
        ITrackerMessageRepository messageRepository,
        ITrackerTagRepository trackerTagRepository, 
        ITrackerRepository trackerRepository,
        Func<MessgingBusType, IMessageBus> busResolver,
        IOptions<RetranslatorOptions> retranslatorOptions)
    {
        _logger = logger;
        _parserProvider = parserProvider;
        _commandCodecProvider = codecProvider;
        _messageRepository = messageRepository;
        _trackerTagRepository = trackerTagRepository;
        _trackerRepository = trackerRepository;
        _retranslatorOptions = retranslatorOptions.Value;
        _consumerBus = busResolver(MessgingBusType.Consuming);
        _retranslatorBus = busResolver(MessgingBusType.Retranslation);
        _commandReceiveBus = busResolver(MessgingBusType.TrackerCommandsReceive);
    }

    public Task HandleAsync(byte[] binaryMessage, ulong deliveryTag)
    {
        var messageText = Encoding.UTF8.GetString(binaryMessage);
        _logger.LogDebug("Получен пакет {MessageText}", messageText);

        var rawMessage = JsonSerializer.Deserialize<RawTrackerMessage>(messageText);
        if (rawMessage == null)
        {
            _consumerBus.Nack(deliveryTag, multiple: false, requeue: false);
            throw new ArgumentException("Невозможно разобрать сырое сообщение", messageText);
        }

        try
        {
            if (_parserProvider(rawMessage.TrackerType).IsCommandReply(rawMessage.RawMessage))
            {
                ProcessCommandReply(rawMessage);
                _consumerBus.Ack(deliveryTag, multiple: false);
                return Task.CompletedTask;
            }

            if (_messageRepository.ExistsByUid(rawMessage.PackageUID))
            {
                _consumerBus.Ack(deliveryTag, multiple: false);
                return Task.CompletedTask;
            }

            var parseStart = DateTime.UtcNow;
            TrackerMessage[] messages = _parserProvider(rawMessage.TrackerType).ParseMessage(rawMessage.RawMessage, rawMessage.PackageUID)
                .ToArray();

            _logger.LogTrace("TrackerMessageHandler after parcing {PackageUID} parsing took {Time} ms", rawMessage.PackageUID, (DateTime.UtcNow - parseStart).TotalMilliseconds);

            Tracker? tracker = null;
            if (messages.Length > 0)
            {
                var firstMessage = messages[0]; // все сообщения в пакете от одного трекера
                tracker = _trackerRepository.FindTracker(firstMessage.Imei, firstMessage.ExternalTrackerId);
            }

            if (tracker is not null)
            {
                tracker.SetTrackerAddress(rawMessage.IpAddress, rawMessage.Port);
                _trackerRepository.Update(tracker);
                foreach (var trackerMessage in messages)
                {
                    trackerMessage.ExternalTrackerId = tracker.ExternalId;
                }
                CalculateSensors(rawMessage.PackageUID, messages, tracker!);
            }
            else
            {
                _logger.LogWarning("Пропущен расчет датчиков из-за того, что не удалось найти трекер");
            }
            foreach (var trackerMessage in messages)
            {
                AddMessageToRepository(rawMessage, trackerMessage);
            }

            if (_retranslatorOptions.Enabled &&
                (_retranslatorOptions.AllowedExtIds is null ||
                 (messages.Length > 0 && _retranslatorOptions.AllowedExtIds.Contains(messages[0].ExternalTrackerId.ToString()))))
            {
                _retranslatorBus.Publish(binaryMessage);
            }
            else
            {
                _logger.LogTrace("Retranslation skipped as {AllowedExtIdsIsNull} and {AllowedExtIds}", _retranslatorOptions.AllowedExtIds is null, _retranslatorOptions.AllowedExtIds);
            }

            _consumerBus.Ack(deliveryTag, multiple: false);
            return Task.CompletedTask;
        }
        catch {
            _consumerBus.Nack(deliveryTag, multiple: false, requeue: true);
            throw;
        }
    }

    private void AddMessageToRepository(RawTrackerMessage rawMessage, TrackerMessage trackerMessage)
    {
        _logger.LogTrace("TrackerMessageHandler перед добавлением нового сообщения из пакета {PackageUID} Id сообщения {MessageId} TrId={TrId}, Imei={Imei}",
            rawMessage.PackageUID, trackerMessage.Id, trackerMessage.ExternalTrackerId, trackerMessage.Imei);
        _messageRepository.Add(trackerMessage);
        _logger.LogDebug("Добавлено новое сообщение из пакета {PackageUID} Id сообщения {MessageId} TrId={TrId}, Imei={Imei}",
            rawMessage.PackageUID, trackerMessage.Id, trackerMessage.ExternalTrackerId, trackerMessage.Imei);
    }

    private void CalculateSensors(Guid packageUID, TrackerMessage[] messages, Tracker tracker)
    {
        _logger.LogTrace("TrackerMessageHandler before calculating tags {PackageUID}", packageUID);
        try
        {
            CalculateSensorTags(tracker, messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при вычислении тегов датчиков");
        }
        _logger.LogTrace("TrackerMessageHandler after calculating tags {PackageUID}", packageUID);
    }

    private void ProcessCommandReply(RawTrackerMessage? rawMessage)
    {
        _logger.LogDebug("Получен ответ на команду RawMessage = {RawMessage}", string.Join(' ', rawMessage.RawMessage.Select(x => x.ToString("X"))));
        CommandResponseInfo responseInfo = _commandCodecProvider(rawMessage.TrackerType).DecodeCommand(rawMessage.RawMessage);
        _logger.LogDebug("Команда декодирована ExternalId={ExternalId}, CommandId={CommandId}, ResponseText={ResponseText}",
            responseInfo.ExternalId, responseInfo.CommandId, responseInfo.ResponseText);
        TrackerCommandResponseMessage commendResponseMessage = new()
        {
            Imei = responseInfo.Imei,
            ExternalId = responseInfo.ExternalId,
            CommandId = responseInfo.CommandId,
            ResponseText = responseInfo.ResponseText,
            ResponseBinary = responseInfo.ResponseBinary,
            ResponseDateTime = SystemTime.UtcNow
        };
        _commandReceiveBus.Publish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(commendResponseMessage)));
    }

    private void CalculateSensorTags(Tracker tracker, TrackerMessage[] messages)
    {
        TrackerMessage? previousMessage = _messageRepository.GetLastMessageFor(tracker.ExternalId);

        TrackerTag[] trackerTags = _trackerTagRepository.GetTags().ToArray();

        if (previousMessage is not null)
        {
            _logger.LogTrace("Перед UpdateSensorTags");
            messages.UpdateSensorTags(new Dictionary<int, TrackerMessage>() { { previousMessage.ExternalTrackerId, previousMessage } },
                new[] { tracker }, trackerTags, new LoggingExceptionHandler(_logger));
        }
    }
}
