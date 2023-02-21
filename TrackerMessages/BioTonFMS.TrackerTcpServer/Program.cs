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

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config/appsettings.json", true);

var serverSettings = builder.Configuration.GetSection("ServerSettings").Get<ServerSettingsOptions>();

builder.Services.Configure<MessageBrokerSettingsOptions>("Primary",
    builder.Configuration.GetSection("PrimaryMessageBrokerSettings"));
builder.Services.Configure<MessageBrokerSettingsOptions>("Secondary",
    builder.Configuration.GetSection("SecondaryMessageBrokerSettings"));
builder.Services.AddSingleton<IMessageBus>(provider =>
{
    var snapshot = provider.GetRequiredService<IOptionsSnapshot<MessageBrokerSettingsOptions>>();
    var primary = new RabbitMQMessageBus(
        provider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
        provider, Options.Create(snapshot.Get("Primary")));
    var secondary = new RabbitMQMessageBus(
        provider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
        provider, Options.Create(snapshot.Get("Secondary")));
    return new MessageBusMux(primary, secondary);
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
    
    if (server != null)
    {
        var addresses = server.Features.Get<IServerAddressesFeature>()!.Addresses;
        Console.WriteLine(String.Join(Environment.NewLine, addresses));
    }
});

app.Run();

