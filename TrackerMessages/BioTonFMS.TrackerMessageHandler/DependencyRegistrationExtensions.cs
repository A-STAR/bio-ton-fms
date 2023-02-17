using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.EF.Providers;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using BioTonFMS.Infrastructure.EF.Repositories.ProtocolTags;
using BioTonFMS.Infrastructure.EF.Repositories.SensorGroups;
using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace BioTonFMS.TrackerMessageHandler;

public static class DependencyRegistrationExtensions
{
    public static IServiceCollection RegisterInfrastructureComponents(this IServiceCollection services)
    {
        services.AddScoped<Factory<BioTonDBContext>>(sp =>
        {
            var instance = sp.GetRequiredService<BioTonDBContext>();
            return () => instance;
        });
        services.AddScoped<Factory<MessagesDBContext>>(sp =>
        {
            var instance = sp.GetRequiredService<MessagesDBContext>();
            return () => instance;
        });

        services.AddTransient<IUnitOfWork<BioTonDBContext>, UnitOfWork<BioTonDBContext>>();
        services.AddTransient<IUnitOfWork<MessagesDBContext>, UnitOfWork<MessagesDBContext>>();

        services.AddTransient(sp =>
        {
            Console.WriteLine("UnitOfWorkFactory<BioTonDBContext> invoked");
            return new UnitOfWorkFactory<BioTonDBContext>(sp);
        });
        services.AddTransient(sp =>
        {
            Console.WriteLine("UnitOfWorkFactory<MessagesDBContext> invoked");
            return new UnitOfWorkFactory<MessagesDBContext>(sp);
        });
        return services;
    }

    public static IServiceCollection RegisterDataAccess(this IServiceCollection services)
    {
        services.AddTransient<IKeyValueProvider<TrackerTag, int>, KeyValueProvider<TrackerTag, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<TrackerTag>, QueryableProvider<TrackerTag, BioTonDBContext>>();
        services.AddTransient<ITrackerTagRepository, TrackerTagRepository>();

        services.AddTransient<IKeyValueProvider<ProtocolTag, int>, KeyValueProvider<ProtocolTag, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<ProtocolTag>, QueryableProvider<ProtocolTag, BioTonDBContext>>();
        services.AddTransient<IProtocolTagRepository, ProtocolTagRepository>();
            
        return services;
    }

    public static IServiceCollection RegisterMessagesDataAccess(this IServiceCollection services)
    {
        services.AddTransient<IKeyValueProvider<TrackerMessage, int>, KeyValueProvider<TrackerMessage, MessagesDBContext, int>>();
        services.AddTransient<IQueryableProvider<TrackerMessage>, QueryableProvider<TrackerMessage, MessagesDBContext>>();
        services.AddTransient<ITrackerMessageRepository, TrackerMessageRepository>();

        return services;
    }
}