using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.TrackerMessageHandler.Workers;

public class MessageHandlerWorker : BackgroundService
{
    private readonly ILogger<MessageHandlerWorker> _logger;
    private readonly IMessageBus _consumerBus;

    public MessageHandlerWorker(
        ILogger<MessageHandlerWorker> logger,
        Func<MessgingBusType, IMessageBus> busResolver
        )
    {
        _logger = logger;
        _consumerBus = busResolver(MessgingBusType.Consuming);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MessageHandlerWorker обработка начата в : {time}", DateTimeOffset.Now);

        _consumerBus.Subscribe<Handlers.TrackerMessageHandler>();
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);

        _logger.LogInformation("MessageHandlerWorker обработка закончена в : {time}", DateTimeOffset.Now);

    }
}



