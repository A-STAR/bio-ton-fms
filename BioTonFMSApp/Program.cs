using BioTonFMS.Infrastructure.EF;
using BioTonFMSApp.Startup;
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
                    .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .UseSnakeCaseNamingConvention());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterInfrastructureComponents();
builder.RegisterDataAccess();

var app = builder.Build();

app.CreateDbIfNotExists();

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
