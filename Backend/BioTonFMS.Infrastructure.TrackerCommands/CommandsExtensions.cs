using BioTonFMS.Common.Settings;
using BioTonFMS.Domain;
using BioTonFMS.TrackerProtocolSpecific.CommandCodecs;
using BioTonFMS.TrackerProtocolSpecific.Senders;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BioTonFMS.TrackerProtocolSpecific;

public static class CommandsExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services, IConfiguration cnf)
    {
        services.Configure<TrackerOptions>(cnf.GetSection("TrackerOptions"));
        
        services.AddSingleton<Func<TrackerTypeEnum, ICommandCodec>>(
            type => type switch
            {
                TrackerTypeEnum.GalileoSkyV50 => new GalileoskyCommandCodec(),
                _ => throw new NotImplementedException()
            });

        services.AddTransient<TcpGalileoskyTrackerCommandSender>();

        services.AddSingleton<Func<TrackerTypeEnum, TrackerCommandTransportEnum, ITrackerCommandSender?>>(
            serviceProvider => (type, transport) => (type, transport) switch
            {
                (TrackerTypeEnum.GalileoSkyV50, TrackerCommandTransportEnum.TCP) =>
                    serviceProvider.GetService<TcpGalileoskyTrackerCommandSender>(),
                _ => (ITrackerCommandSender?)null
            });

        return services;
    }
}