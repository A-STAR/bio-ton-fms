using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Hosting;

public class CommandResponseWorker : BackgroundService
{
    private readonly ILogger<CommandResponseWorker> _logger;
    private readonly IMessageBus _commandsReceiveBus;

    public CommandResponseWorker(
        ILogger<CommandResponseWorker> logger,
        Func<MessgingBusType, IMessageBus> busResolver
    )
    {
        _logger = logger;
        _commandsReceiveBus = busResolver.Invoke(MessgingBusType.TrackerCommandsReceive);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CommandResponseWorker обработка начата в : {time}", DateTimeOffset.Now);

        _commandsReceiveBus.Subscribe<CommandResponseHandler>();
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);

        _logger.LogInformation("CommandResponseWorker обработка закончена в : {time}", DateTimeOffset.Now);

    }
}