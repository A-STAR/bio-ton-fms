using System.Text;
using System.Text.Json;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.TrackerMessageHandler.MessageParsing;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.TrackerMessageHandler;

public class TrackerMessageHandler : IBusMessageHandler
{
    private readonly ILogger<TrackerMessageHandler> _logger;
    private readonly Func<TrackerTypeEnum, IMessageParser> _parserProvider;

    public TrackerMessageHandler(ILogger<TrackerMessageHandler> logger,
        Func<TrackerTypeEnum, IMessageParser> parserProvider)
    {
        _logger = logger;
        _parserProvider = parserProvider;
    }

    public Task HandleAsync(byte[] message)
    {
        var messageText = Encoding.UTF8.GetString(message);
        _logger.LogInformation("Получено сообщение {MessageText}", messageText);

        var rawMessage = JsonSerializer.Deserialize<RawTrackerMessage>(messageText)
                         ?? throw new ArgumentException("Невозможно разобрать сырое сообщение", nameof(message));

        _parserProvider(rawMessage.TrackerType).ParseMessage(rawMessage.RawMessage);
            
        return Task.CompletedTask;
    }
}