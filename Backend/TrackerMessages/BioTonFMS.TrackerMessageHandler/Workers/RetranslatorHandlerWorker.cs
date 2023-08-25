using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.TrackerMessageHandler.Workers;

public class RetranslatorHandlerWorker : BackgroundService
{
    private readonly ILogger<RetranslatorHandlerWorker> _logger;
    private readonly IMessageBus _retranslationBus;

    public RetranslatorHandlerWorker(
        ILogger<RetranslatorHandlerWorker> logger,
        Func<MessgingBusType, IMessageBus> busResolver
    )
    {
        _logger = logger;
        _retranslationBus = busResolver(MessgingBusType.Retranslation);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RetranslatorHandlerWorker обработка начата в : {time}", DateTimeOffset.Now);

        _retranslationBus.Subscribe<Handlers.RetranslatorHandler>();
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);

        _logger.LogInformation("RetranslatorHandlerWorker обработка закончена в : {time}", DateTimeOffset.Now);
    }
}