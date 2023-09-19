using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.TrackerProtocolSpecific.CommandCodecs;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Hosting;

public class CommandResponseHandler : IBusMessageHandler
{
    private readonly ITrackerCommandRepository _trackerCommandRepository;
    private readonly ICommandCodec _codec;
    private readonly IMessageBus _commandsReceiveBus;
    private readonly ILogger<CommandResponseHandler> _logger;

    public CommandResponseHandler(
        ITrackerCommandRepository trackerCommandRepository, 
        Func<TrackerTypeEnum, ICommandCodec> codec,
        Func<MessgingBusType, IMessageBus> busResolver,
        ILogger<CommandResponseHandler> logger)
    {
        _trackerCommandRepository = trackerCommandRepository;
        _codec = codec(TrackerTypeEnum.GalileoSkyV50);
        _commandsReceiveBus = busResolver.Invoke(MessgingBusType.TrackerCommandsReceive);
        _logger = logger;
    }

    public async Task HandleAsync(byte[] binaryMessage, ulong deliveryTag)
    {
        try
        {
            _logger.LogDebug("CommandResponseHandler получено сообщение {Message}", string.Join(' ', binaryMessage.Select(x => x.ToString("X"))));

            string msgText = Encoding.UTF8.GetString(binaryMessage);

            TrackerCommandResponseMessage? msg = JsonSerializer.Deserialize<TrackerCommandResponseMessage>(msgText);

            if (msg == null)
            {
                _logger.LogError("Текст сообщения {Text} не соответствует формату TrackerCommandResponseMessage", msgText);
                throw new ArgumentException("Не удалось разобрать команду", nameof(binaryMessage));
            }
            _logger.LogDebug("CommandResponseHandler сообщение CommandId = {CommandId}, ResponseText = {ResponseText}, ResponseDateTime = {ResponseDateTime}",
                msg.CommandId, msg.ResponseText, msg.ResponseDateTime);

            TrackerCommand? command = _trackerCommandRepository[msg.CommandId];

            if (command == null)
            {
                _logger.LogInformation("Команда {Text} была получена, но id = {Id} не существует", msgText, msg.CommandId);
                throw new Exception($"Команда трекеру с id = {msg.CommandId} не найдена");
            }
            command.ResponseText = msg.ResponseText;
            command.BinaryResponse = msg.ResponseBinary;
            command.ResponseDateTime = msg.ResponseDateTime;
            _trackerCommandRepository.Update(command);
            _commandsReceiveBus.Ack(deliveryTag, multiple: false);
        }
        catch
        {
            _commandsReceiveBus.Nack(deliveryTag, multiple: false, requeue: false);
            throw;
        }
    }
}