using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;

namespace BioTonFMS.TrackerMessageHandler;

public class MessageHandlerWorker : BackgroundService
{
    private readonly ILogger<MessageHandlerWorker> _logger;
    private readonly IMessageBus _messageBus;
    private readonly GalileoSkyMessageHandler _galileoSkyMessageHandler;

    public MessageHandlerWorker(
        ILogger<MessageHandlerWorker> logger,
        GalileoSkyMessageHandler galileoSkyMessageHandler,
        IMessageBus messageBus
        )
    {
        _logger = logger;
        _messageBus = messageBus;
        _galileoSkyMessageHandler = galileoSkyMessageHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MessageHandlerWorker обработка начата в : {time}", DateTimeOffset.Now);

        _messageBus.Subscribe<BasicDeliverEventArgs>(_galileoSkyMessageHandler.EventHandler);
        while (!stoppingToken.IsCancellationRequested)
        {
        }

        _logger.LogInformation("MessageHandlerWorker обработка закончена в : {time}", DateTimeOffset.Now);

    }
}



