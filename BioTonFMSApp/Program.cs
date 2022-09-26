using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMSApp.Startup;
using BioTonFMSApp.Startup.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

IConfiguration configuration = new ConfigurationBuilder()
#if DEBUG                
                .AddJsonFile("appsettings.Development.json", true)
#else
                .AddJsonFile("appsettings.json")
#endif
   .Build();

builder.Services.AddDbContext<BioTonDBContext>(
        options => options
                    .UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
                        x => x.MigrationsAssembly("BioTonFMS.Migrations"))
                    .UseSnakeCaseNamingConvention());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterInfrastructureComponents();
builder.RegisterDataAccess();

builder.AddAuth();
builder.AddValidation();
builder.AddSwagger();

// configuring Serilog
builder.ConfigureSerilog();

var app = builder.Build();

await app.ApplyMigrationsAsync(builder.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
