using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using BioTonFMS.TrackerTcpServer;
using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.Infrastructure.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

var serverSettings = builder.Configuration.GetSection("ServerSettings").Get<ServerSettingsOptions>();

builder.WebHost.UseKestrel(so =>
{
    so.Listen(new IPEndPoint(IPAddress.Parse(serverSettings.IpAddress), serverSettings.GalileoskyPort),
      epo => epo.UseConnectionHandler<GalileoTrackerConnectionHandler>());
});
builder.Services.AddOptions();
builder.Services.Configure<MessageBrokerSettingsOptions>(builder.Configuration.GetSection("MessageBrokerSettings"));
builder.Services.AddTransient<IMessageBus, RabbitMQMessageBus>();
var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var server = app.Services.GetRequiredService<IServer>();
    if (server != null)
    {
        var addresses = server.Features.Get<IServerAddressesFeature>()!.Addresses;
        Console.WriteLine(String.Join(Environment.NewLine, addresses));
    }

});
app.Run();

