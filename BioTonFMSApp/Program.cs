using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Security.Controllers;
using BioTonFMS.Telematica.Controllers;
using BioTonFMSApp.Startup;
using BioTonFMSApp.Startup.Swagger;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config/appsettings.json", true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
})
.AddApplicationPart(typeof(AuthController).Assembly)
.AddApplicationPart(typeof(TrackerController).Assembly);

builder.Services.AddOptions();
builder.Services.Configure<VersionInfoOptions>(builder.Configuration.GetSection("ShowVersionOptions"));

builder.Services.AddDbContext<BioTonDBContext>(
        options => options
                    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("BioTonFMS.Migrations"))
                    .UseSnakeCaseNamingConvention()
                    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.AddMappingProfiles();
builder.RegisterInfrastructureComponents();
builder.RegisterDataAccess();

builder.AddAuth();
builder.AddValidation();
builder.AddSwagger();

builder.ConfigureSerilog();

var app = builder.Build();

await app.ApplyMigrationsAsync(builder.Configuration);

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

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
