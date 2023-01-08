using RabbitMQ.Client.Events;
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
        _logger.LogInformation("MessageHandlerWorker ��������� ������ � : {time}", DateTimeOffset.Now);

        _messageBus.Subscribe<TrackerMessageHandler>();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Yield();
        }

        _logger.LogInformation("MessageHandlerWorker ��������� ��������� � : {time}", DateTimeOffset.Now);

    }
}



