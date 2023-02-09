using BioTonFMS.Common.Settings;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.Infrastructure.RabbitMQ;
using BioTonFMS.Migrations;
using BioTonFMS.Telematica.MessageParsing;
using BioTonFMS.TrackerMessageHandler;
using BioTonFMSApp.Startup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions();
        services.Configure<MessageBrokerSettingsOptions>(hostContext.Configuration.GetSection("MessageBrokerSettings"));
        services.AddTransient<TrackerMessageHandler>();
        services.AddSingleton<IMessageBus, RabbitMQMessageBus>();
        services.AddHostedService<MessageHandlerWorker>();
        services.AddOptions<DbMigrationOptions>("DbMigrationOptions");

        services.AddTransient<GalileoskyMessageParser>();
        services.AddTransient<Func<TrackerTypeEnum, IMessageParser>>(provider => key => key switch
        {
            TrackerTypeEnum.GalileoSkyV50 => provider.GetRequiredService<GalileoskyMessageParser>(),
            TrackerTypeEnum.Retranslator => throw new NotImplementedException(),
            TrackerTypeEnum.WialonIPS => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        });

        services.AddDbContext<MessagesDBContext>(
            options => options
                .UseNpgsql(hostContext.Configuration.GetConnectionString("MessagesConnection")!,
                    x => x.MigrationsAssembly("BioTonFMS.MessagesMigrations"))
                .UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging(hostContext.HostingEnvironment.IsDevelopment()));
        
        services.AddDbContext<BioTonDBContext>(
            options => options
                .UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")!,
                    x => x.MigrationsAssembly("BioTonFMS.Migrations"))
                .UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging(hostContext.HostingEnvironment.IsDevelopment()));
        
        services.AddMappingProfiles()
            .RegisterInfrastructureComponents()
            .RegisterDataAccess()
            .RegisterMessagesDataAccess();
    })
    .UseSerilog((hostContext, config) =>
    {
        config.ReadFrom.Configuration(hostContext.Configuration)
            .Enrich.FromLogContext();
    })
    .Build();

await Migrate(host);

await host.RunAsync();

async Task Migrate(IHost h)
{
    using var scope = h.Services.CreateScope();
    var services = scope.ServiceProvider;

    if (!services.GetRequiredService<IOptions<DbMigrationOptions>>().Value.ApplyMigrations)
        return;

    var msgContext = services.GetRequiredService<MessagesDBContext>();
    await msgContext.Database.MigrateAsync();
    
    var context = services.GetRequiredService<BioTonDBContext>();
    await context.Database.MigrateAsync();
}