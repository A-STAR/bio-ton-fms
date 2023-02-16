using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TestTcpTrackerClient;

var host = Host.CreateDefaultBuilder()
    .ConfigureHostConfiguration(x => x.AddJsonFile("appsettings.json"))
    .ConfigureServices((context, services) =>
    {
        services.Configure<ClientOptions>(context.Configuration.GetSection("ClientOptions"));
        AddClientParams(services, args);
        services.AddHostedService<Worker>();
    })
    .UseSerilog((context, config) => config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
    )
    .Build();

await host.RunAsync();

void AddClientParams(IServiceCollection services, string[] args)
{
    services.AddSingleton(args.Length switch
    {
        1 => new ClientParams
        {
            ScriptPath = args[0]
        },
        2 => new ClientParams
        {
            InputPath = args[0],
            OutputPath = args[1]
        },
        3 => new ClientParams
        {
            InputPath = args[0],
            OutputPath = args[1],
            RepeatCount = int.Parse(args[3])
        },
        _ => throw new Exception(Constants.Instruction)
    });
}