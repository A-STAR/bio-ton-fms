using System.Text;
using System.Text.Json;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.MessageBus;


namespace BioTonFMS.TrackerTcpServer;

public class TcpSendCommandMessageHandler : IBusMessageHandler
{
    private readonly ILogger<TcpSendCommandMessageHandler> _logger;
    private readonly TcpSendCommandMessages _commandMessages;
    private readonly IMessageBus _trackerCommandBus;

    public TcpSendCommandMessageHandler(
        ILogger<TcpSendCommandMessageHandler> logger,
        TcpSendCommandMessages commandMessages,
        Func<MessgingBusType, IMessageBus> busResolver
    )
    {
        _logger = logger;
        _commandMessages = commandMessages;
        _trackerCommandBus = busResolver(MessgingBusType.TrackerCommandsSend);
    }

    public Task HandleAsync(byte[] binaryMessage, ulong deliveryTag)
    {
        try
        {
            var messageText = Encoding.UTF8.GetString(binaryMessage);
            _logger.LogDebug("TcpCommandMessageHandler Получен пакет {MessageText}", messageText);

            var commandMessage = JsonSerializer.Deserialize<TrackerCommandMessage>(messageText)
                ?? throw new ArgumentException("Невозможно разобрать сырое сообщение с командой", messageText);
            _commandMessages.AddSendCommandMessage(commandMessage);
            _logger.LogDebug("TcpCommandMessageHandler сообщение положено в очередь");

            _trackerCommandBus.Ack(deliveryTag, multiple: false);
            return Task.CompletedTask;
        }
        catch
        {
            _trackerCommandBus.Nack(deliveryTag, multiple: false, requeue: false);
            throw;
        }
    }


}
