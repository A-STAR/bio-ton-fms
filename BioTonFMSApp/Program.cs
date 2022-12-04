using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Security.Controllers;
using BioTonFMS.Telematica.Controllers;
using BioTonFMSApp.Startup;
using BioTonFMSApp.Startup.Swagger;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Text;
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

// ��� ���������� ���������� ����� ��������� ����������� ��� ������� �������
// �� �������� ������ � ������� ������
string locale = "ru-RU";
RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
{
    SupportedCultures = new List<CultureInfo> { new CultureInfo(locale) },
    SupportedUICultures = new List<CultureInfo> { new CultureInfo(locale) },
    DefaultRequestCulture = new RequestCulture(locale)
};
app.UseRequestLocalization(localizationOptions);

app.MapControllers();

app.Run();
