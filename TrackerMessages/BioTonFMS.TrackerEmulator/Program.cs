using BioTonFMS.TrackerEmulator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

if (args.Length == 0)
{
    Console.WriteLine(Constants.Instruction);
    return;
}

var host = Host.CreateDefaultBuilder()
    .ConfigureHostConfiguration(x =>
    {
        x.AddCommandLine(args, new Dictionary<string, string>
        {
            {"-r", "repeat"},
            {"-i", "message"},
            {"-o", "result"},
            {"-s", "script"},
        }).Build();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<ClientOptions>(context.Configuration.GetSection("ClientOptions"));
        AddClientParams(services, context.Configuration);
        services.AddHostedService<Worker>();
    })
    .UseSerilog((context, config) => config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
    )
    .Build();

await host.RunAsync();

void AddClientParams(IServiceCollection services, IConfiguration config)
{
    if (!int.TryParse(config["repeat"], out int repeatCnt)) repeatCnt = 1;

    services.AddSingleton(new ClientParams
    {
        MessagePath = config["message"],
        ResultPath = config["result"],
        RepeatCount = repeatCnt,
        ScriptPath = config["script"]
    });
}