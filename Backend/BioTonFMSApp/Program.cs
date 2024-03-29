using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.Infrastructure.RabbitMQ;
using BioTonFMS.Security.Controllers;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Hosting;
using BioTonFMS.TrackerProtocolSpecific;
using BioTonFMSApp.Scheduler;
using BioTonFMSApp.Startup;
using BioTonFMSApp.Startup.Swagger;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "../../frontend/dist/bio-ton-field-management-system"
});
builder.Configuration.AddJsonFile("config/appsettings.json", true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
})
.AddApplicationPart(typeof(AuthController).Assembly)
.AddApplicationPart(typeof(TrackerController).Assembly);

builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<Func<MessgingBusType, IMessageBus>>(serviceProvider => busType =>
    MessageBusFactory.CreateOrGetBus(busType, serviceProvider)
);

builder.Services.AddOptions();
builder.Services.Configure<VersionInfoOptions>(builder.Configuration.GetSection("ShowVersionOptions"));

builder.Services.AddDbContext<BioTonDBContext>(
        options => options
                    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("BioTonFMS.Migrations"))
                    .UseSnakeCaseNamingConvention()
                    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services.AddDbContext<MessagesDBContext>(
    options => options
        .UseNpgsql(builder.Configuration.GetConnectionString("MessagesConnection"),
            x => x.MigrationsAssembly("BioTonFMS.MessagesMigrations"))
        .UseSnakeCaseNamingConvention()
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services.AddTransient<CommandResponseHandler>();
builder.Services.AddCommands(builder.Configuration);
builder.Services.AddHostedService<CommandResponseWorker>();

builder.Services
    .AddMappingProfiles()
    .RegisterInfrastructureComponents()
    .RegisterDataAccess()
    .RegisterMessagesDataAccess()
    .RegisterServices()
    .RegisterSchedulerJobs();

builder.AddAuth();
builder.AddValidation();
builder.AddSwagger();

builder.ConfigureSerilog();

var app = builder.Build();

Scheduler.Init(app.Services);

await app.ApplyMigrationsAsync(builder.Configuration);

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

// Для приложения выставляем явные параметры локализации для каждого запроса
// Мы работаем только с русским языком.
const string locale = "ru-RU";
var localizationOptions = new RequestLocalizationOptions
{
    SupportedCultures = new List<CultureInfo> { new CultureInfo(locale) },
    SupportedUICultures = new List<CultureInfo> { new CultureInfo(locale) },
    DefaultRequestCulture = new RequestCulture(locale)
};
app.UseRequestLocalization(localizationOptions);

app.MapControllers();

app.Run();

//