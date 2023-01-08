using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.Infrastructure.RabbitMQ;
using BioTonFMS.TrackerMessageHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions();
        services.Configure<MessageBrokerSettingsOptions>(hostContext.Configuration.GetSection("MessageBrokerSettings"));
        services.AddTransient<TrackerMessageHandler>();
        services.AddSingleton<IMessageBus, RabbitMQMessageBus>();
        services.AddHostedService<MessageHandlerWorker>();
    })
    .Build();

await host.RunAsync();

