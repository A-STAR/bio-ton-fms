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

builder.Services.Configure<MessageBrokerSettingsOptions>("Primary",
    builder.Configuration.GetSection("PrimaryMessageBrokerSettings"));
builder.Services.Configure<MessageBrokerSettingsOptions>("Secondary",
    builder.Configuration.GetSection("SecondaryMessageBrokerSettings"));
builder.Services.Configure<RetryOptions>(
    builder.Configuration.GetSection("RetryOptions"));
builder.Services.AddSingleton<IMessageBus>(serviceProvider =>
{
    var timeouts = serviceProvider.GetRequiredService<IOptions<RetryOptions>>()
        .Value.TimeoutsInMs.Select(x => TimeSpan.FromSeconds(x));
    var monitor = serviceProvider.GetRequiredService<IOptionsMonitor<MessageBrokerSettingsOptions>>();
    var primary = new RabbitMQMessageBus(
        serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
        serviceProvider, Options.Create(monitor.Get("Primary")));

    var secondaryOptions = Options.Create(monitor.Get("Secondary"));
    RabbitMQMessageBus? secondary = null;
    if (secondaryOptions.Value.Enabled)
    {
        secondary = new RabbitMQMessageBus(
            serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
            serviceProvider, secondaryOptions);
    }
    return new MessageBusMux(primary, secondary, Policy.Handle<Exception>().WaitAndRetry(timeouts));
});
builder.Services.AddTransient<GalileoskyProtocolMessageHandler>();
builder.Services.AddTransient<Func<TrackerTypeEnum, IProtocolMessageHandler>>(provider => key => key switch
{
    TrackerTypeEnum.GalileoSkyV50 => provider.GetRequiredService<GalileoskyProtocolMessageHandler>(),
    TrackerTypeEnum.Retranslator => throw new NotImplementedException(),
    TrackerTypeEnum.WialonIPS => throw new NotImplementedException(),
    _ => throw new NotImplementedException()
});

builder.ConfigureSerilog();

builder.WebHost.UseKestrel(so =>
{
    so.Listen(new IPEndPoint(IPAddress.Parse(serverSettings.IpAddress), serverSettings.GalileoskyPort),
        epo => epo.UseConnectionHandler<GalileoTrackerConnectionHandler>());
});

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var server = app.Services.GetService<IServer>();
    
    if (server is not null)
    {
        var addresses = server.Features.Get<IServerAddressesFeature>()!.Addresses;
        Console.WriteLine(String.Join(Environment.NewLine, addresses));
    }
});

app.Run();

