using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BioTonFMS.Infrastructure.MessageBus;

namespace BioTonFMS.TrackerMessageHandler;

public class MessageHandlerWorker : BackgroundService
{
    private readonly ILogger<MessageHandlerWorker> _logger;
    private readonly IMessageBus _messageBus;

    public MessageHandlerWorker(
        ILogger<MessageHandlerWorker> logger,
        IMessageBus messageBus
        )
    {
        _logger = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MessageHandlerWorker обработка начата в : {time}", DateTimeOffset.Now);

        _messageBus.Subscribe<TrackerMessageHandler>();
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);

        _logger.LogInformation("MessageHandlerWorker обработка закончена в : {time}", DateTimeOffset.Now);

    }
}



