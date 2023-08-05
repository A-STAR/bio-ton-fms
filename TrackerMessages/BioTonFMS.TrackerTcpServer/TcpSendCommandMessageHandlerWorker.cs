using BioTonFMS.Infrastructure.MessageBus;

namespace BioTonFMS.TrackerTcpServer;

public class TcpSendCommandMessageHandlerWorker : BackgroundService
{
    private readonly ILogger<TcpSendCommandMessageHandlerWorker> _logger;
    private readonly IMessageBus _trackerCommandBus;

    public TcpSendCommandMessageHandlerWorker(
        ILogger<TcpSendCommandMessageHandlerWorker> logger,
        Func<MessgingBusType, IMessageBus>  busResolver
        )
    {
        _logger = logger;
        _trackerCommandBus = busResolver(MessgingBusType.TrackerCommandsSend);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TcpCommandMessageHandlerWorker обработка начата в : {time}", DateTimeOffset.Now);

        _trackerCommandBus.Subscribe<TcpSendCommandMessageHandler>();
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);

        _logger.LogInformation("TcpCommandMessageHandlerWorker обработка закончена в : {time}", DateTimeOffset.Now);

    }
}



