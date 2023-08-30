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
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BioTonFMS.Telematica.Services;
using BioTonFMSApp.Scheduler;

namespace BioTonFMSApp.Startup;

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
        services.AddTransient<IKeyValueProvider<Tracker, int>, KeyValueProvider<Tracker, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<Tracker>, QueryableProvider<Tracker, BioTonDBContext>>();
        services.AddTransient<ITrackerRepository, TrackerRepository>();

        services.AddTransient<IKeyValueProvider<Vehicle, int>, KeyValueProvider<Vehicle, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<Vehicle>, QueryableProvider<Vehicle, BioTonDBContext>>();
        services.AddTransient<IVehicleRepository, VehicleRepository>();

        services.AddTransient<IKeyValueProvider<VehicleGroup, int>, KeyValueProvider<VehicleGroup, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<VehicleGroup>, QueryableProvider<VehicleGroup, BioTonDBContext>>();
        services.AddTransient<IVehicleGroupRepository, VehicleGroupRepository>();

        services.AddTransient<IKeyValueProvider<FuelType, int>, KeyValueProvider<FuelType, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<FuelType>, QueryableProvider<FuelType, BioTonDBContext>>();
        services.AddTransient<IFuelTypeRepository, FuelTypeRepository>();

        services.AddTransient<IKeyValueProvider<Sensor, int>, KeyValueProvider<Sensor, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<Sensor>, QueryableProvider<Sensor, BioTonDBContext>>();
        services.AddTransient<ISensorRepository, SensorRepository>();

        services.AddTransient<IKeyValueProvider<SensorType, int>, KeyValueProvider<SensorType, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<SensorType>, QueryableProvider<SensorType, BioTonDBContext>>();
        services.AddTransient<ISensorTypeRepository, SensorTypeRepository>();

        services.AddTransient<IKeyValueProvider<SensorGroup, int>, KeyValueProvider<SensorGroup, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<SensorGroup>, QueryableProvider<SensorGroup, BioTonDBContext>>();
        services.AddTransient<ISensorGroupRepository, SensorGroupRepository>();

        services.AddTransient<IKeyValueProvider<Unit, int>, KeyValueProvider<Unit, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<Unit>, QueryableProvider<Unit, BioTonDBContext>>();
        services.AddTransient<IUnitRepository, UnitRepository>();

        services.AddTransient<IKeyValueProvider<TrackerTag, int>, KeyValueProvider<TrackerTag, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<TrackerTag>, QueryableProvider<TrackerTag, BioTonDBContext>>();
        services.AddTransient<ITrackerTagRepository, TrackerTagRepository>();

        services.AddTransient<IKeyValueProvider<ProtocolTag, int>, KeyValueProvider<ProtocolTag, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<ProtocolTag>, QueryableProvider<ProtocolTag, BioTonDBContext>>();
        services.AddTransient<IProtocolTagRepository, ProtocolTagRepository>();

        services.AddTransient<IKeyValueProvider<TrackerCommand, int>, KeyValueProvider<TrackerCommand, BioTonDBContext, int>>();
        services.AddTransient<IQueryableProvider<TrackerCommand>, QueryableProvider<TrackerCommand, BioTonDBContext>>();
        services.AddTransient<ITrackerCommandRepository, TrackerCommandRepository>();

        return services;
    }

    public static IServiceCollection RegisterMessagesDataAccess(this IServiceCollection services)
    {
        services.AddTransient<IKeyValueProvider<TrackerMessage, int>, KeyValueProvider<TrackerMessage, MessagesDBContext, int>>();
        services.AddTransient<IQueryableProvider<TrackerMessage>, QueryableProvider<TrackerMessage, MessagesDBContext>>();
        services.AddTransient<ITrackerMessageRepository, TrackerMessageRepository>();

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<MoveTestTrackerMessagesService, MoveTestTrackerMessagesService>();

        return services;
    }

    public static IServiceCollection RegisterSchedulerJobs(this IServiceCollection services)
    {
        services.AddTransient<MoveTestTrackerMessagesJob, MoveTestTrackerMessagesJob>();

        return services;
    }
    
}