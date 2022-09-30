using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMSApp.Startup;
using BioTonFMSApp.Startup.Swagger;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddMvc().AddApplicationPart(Assembly.Load(new AssemblyName("BioTonFMS.Security")));

builder.Services.AddDbContext<BioTonDBContext>(
        options => options
                    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("BioTonFMS.Migrations"))
                    .UseSnakeCaseNamingConvention());

builder.RegisterInfrastructureComponents();
builder.RegisterDataAccess();

builder.AddAuth();
builder.AddValidation();
builder.AddSwagger();

builder.ConfigureSerilog();

var app = builder.Build();

await app.ApplyMigrationsAsync(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
