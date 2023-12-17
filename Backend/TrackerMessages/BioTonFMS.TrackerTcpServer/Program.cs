using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using BioTonFMS.TrackerTcpServer;
using BioTonFMS.Common.Settings;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.Infrastructure.RabbitMQ;
using BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;
using Microsoft.Extensions.Options;
using Polly;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config/appsettings.json", true);

var serverSettings = builder.Configuration.GetSection("ServerSettings").Get<ServerSettingsOptions>();

builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RetryOptions>(builder.Configuration.GetSection("RetryOptions"));
builder.Services.AddTransient<TcpSendCommandMessageHandler>();

builder.Services.AddSingleton<Func<MessgingBusType, IMessageBus>>(serviceProvider => busType =>
    MessageBusFactory.CreateOrGetBus(busType, serviceProvider, GetRawTrackerMessageBus)
);

builder.Services.AddSingleton<TcpSendCommandMessages>();

builder.Services.AddHostedService<TcpSendCommandMessageHandlerWorker>();

builder.Services.AddTransient<GalileoskyProtocolMessageHandler>();
builder.Services.AddTransient<Func<TrackerTypeEnum, IProtocolMessageHandler>>(provider => key => key switch
{
    TrackerTypeEnum.GalileoSkyV50 => provider.GetRequiredService<GalileoskyProtocolMessageHandler>(),
    TrackerTypeEnum.Retranslator => throw new NotImplementedException(),
    TrackerTypeEnum.WialonIPS => throw new NotImplementedException(),
    _ => throw new NotImplementedException()
});

builder.ConfigureSerilog();

Console.WriteLine("test output1");
builder.WebHost.UseKestrel(so =>
{
    so.Listen(new IPEndPoint(IPAddress.Parse(serverSettings.IpAddress), serverSettings.GalileoskyPort),
        epo => epo.UseConnectionHandler<GalileoTrackerConnectionHandler>());
    Console.WriteLine("test output2");
});

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var server = app.Services.GetService<IServer>();
    
    if (server is not null)
    {
        Console.WriteLine("test output2");
        ICollection<string>? addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
        Console.WriteLine(String.Join(Environment.NewLine, addresses??new List<string>()));
    }
});
app.Run();

IMessageBus GetRawTrackerMessageBus(IServiceProvider serviceProvider, RabbitMQOptions rabbitOptions)
{
    var timeouts = serviceProvider.GetRequiredService<IOptions<RetryOptions>>()
        .Value.TimeoutsInMs.Select(x => TimeSpan.FromSeconds(x));
    var rabbitMQOptions = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();
    var primaryBus = new RabbitMQMessageBus(
        serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
        serviceProvider,
        rabbitMQOptions,
        isDurable: true,
        "RawTrackerMessages-primary", 
        needDeadMessageQueue: true,
        deliveryLimit: rabbitOptions.DeliveryLimit,
        queueMaxLength: rabbitOptions.TrackerQueueMaxLength);
    var secondaryOptions = builder.Configuration.GetSection("SecondaryMessageBrokerSettings").Get<MessageBrokerSettingsOptions>();
    RabbitMQMessageBus? secondary = null;
    if (secondaryOptions != null && secondaryOptions.Enabled)
    {
        secondary = new RabbitMQMessageBus(
            serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
            serviceProvider,
            rabbitMQOptions,
            isDurable: true,
            "RawTrackerMessages-secondary",
            needDeadMessageQueue: true,
        deliveryLimit: rabbitOptions.DeliveryLimit);
    }

    return new MessageBusMux(primaryBus, secondary, Policy.Handle<Exception>().WaitAndRetry(timeouts));
}

public partial class Program { }