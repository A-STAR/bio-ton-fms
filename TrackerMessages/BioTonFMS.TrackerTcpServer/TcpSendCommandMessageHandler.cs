using System.Text;
using System.Text.Json;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.MessageBus;


namespace BioTonFMS.TrackerTcpServer;

public class TcpSendCommandMessageHandler : IBusMessageHandler
{
    private readonly ILogger<TcpSendCommandMessageHandler> _logger;
    private readonly TcpSendCommandMessages _commandMessages;

    public TcpSendCommandMessageHandler(
        ILogger<TcpSendCommandMessageHandler> logger,
        TcpSendCommandMessages commandMessages
    )
    {
        _logger = logger;
        _commandMessages = commandMessages;
    }

    public Task HandleAsync(byte[] binaryMessage)
    {
        var messageText = Encoding.UTF8.GetString(binaryMessage);
        _logger.LogDebug("TcpCommandMessageHandler Получен пакет {MessageText}", messageText);

        var commandMessage = JsonSerializer.Deserialize<TrackerCommandMessage>(messageText)
            ?? throw new ArgumentException("Невозможно разобрать сырое сообщение с командой", messageText);
        _commandMessages.AddSendCommandMessage(commandMessage);
        _logger.LogDebug("TcpCommandMessageHandler сообщение положено в очередь");

        return Task.CompletedTask;
    }


}
