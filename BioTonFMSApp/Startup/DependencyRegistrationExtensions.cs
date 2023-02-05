using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.EF.Providers;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using BioTonFMS.Infrastructure.EF.Repositories.SensorGroups;
using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;

namespace BioTonFMSApp.Startup
{
    public static class DependencyRegistrationExtensions
    {
        public static WebApplicationBuilder RegisterInfrastructureComponents(this WebApplicationBuilder builder)
        {
            /*builder.Services.AddScoped<Factory<DbContext>>(sp =>
            {
                var instance = sp.GetRequiredService<BioTonDBContext>();
                return () => instance;
            });*/
            builder.Services.AddScoped<Factory<BioTonDBContext>>(sp =>
            {
                var instance = sp.GetRequiredService<BioTonDBContext>();
                return () => instance;
            });
            builder.Services.AddScoped<Factory<MessagesDBContext>>(sp =>
            {
                var instance = sp.GetRequiredService<MessagesDBContext>();
                return () => instance;
            });

            builder.Services.AddScoped<Factory<DbContext[]>>(sp =>
            {
                var contexts = new DbContext[2];
                contexts[0] = sp.GetRequiredService<BioTonDBContext>();
                contexts[1] = sp.GetRequiredService<MessagesDBContext>();

                return () => contexts;
            });

            builder.Services.AddTransient<IUnitOfWork<BioTonDBContext>, UnitOfWork<BioTonDBContext>>();
            builder.Services.AddTransient<IUnitOfWork<MessagesDBContext>, UnitOfWork<MessagesDBContext>>();

            builder.Services.AddTransient(sp =>
            {
                Console.WriteLine("UnitOfWorkFactory<BioTonDBContext> invoked");
                return new UnitOfWorkFactory<BioTonDBContext>(sp);
            });
            builder.Services.AddTransient(sp =>
            {
                Console.WriteLine("UnitOfWorkFactory<MessagesDBContext> invoked");
                return new UnitOfWorkFactory<MessagesDBContext>(sp);
            });
            return builder;
        }

        public static WebApplicationBuilder RegisterDataAccess(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IKeyValueProvider<Tracker, int>, KeyValueProvider<Tracker, BioTonDBContext, int>>();
            builder.Services.AddTransient<IQueryableProvider<Tracker>, QueryableProvider<Tracker, BioTonDBContext>>();
            builder.Services.AddTransient<ITrackerRepository, TrackerRepository>();

            builder.Services.AddTransient<IKeyValueProvider<Vehicle, int>, KeyValueProvider<Vehicle, BioTonDBContext, int>>();
            builder.Services.AddTransient<IQueryableProvider<Vehicle>, QueryableProvider<Vehicle, BioTonDBContext>>();
            builder.Services.AddTransient<IVehicleRepository, VehicleRepository>();

            builder.Services.AddTransient<IKeyValueProvider<VehicleGroup, int>, KeyValueProvider<VehicleGroup, BioTonDBContext, int>>();
            builder.Services.AddTransient<IQueryableProvider<VehicleGroup>, QueryableProvider<VehicleGroup, BioTonDBContext>>();
            builder.Services.AddTransient<IVehicleGroupRepository, VehicleGroupRepository>();

            builder.Services.AddTransient<IKeyValueProvider<FuelType, int>, KeyValueProvider<FuelType, BioTonDBContext, int>>();
            builder.Services.AddTransient<IQueryableProvider<FuelType>, QueryableProvider<FuelType, BioTonDBContext>>();
            builder.Services.AddTransient<IFuelTypeRepository, FuelTypeRepository>();

            builder.Services.AddTransient<IKeyValueProvider<Sensor, int>, KeyValueProvider<Sensor, BioTonDBContext, int>>();
            builder.Services.AddTransient<IQueryableProvider<Sensor>, QueryableProvider<Sensor, BioTonDBContext>>();
            builder.Services.AddTransient<ISensorRepository, SensorRepository>();

            builder.Services.AddTransient<IKeyValueProvider<SensorType, int>, KeyValueProvider<SensorType, int>>();
            builder.Services.AddTransient<IQueryableProvider<SensorType>, QueryableProvider<SensorType>>();
            builder.Services.AddTransient<ISensorTypeRepository, SensorTypeRepository>();

            builder.Services.AddTransient<IKeyValueProvider<SensorGroup, int>, KeyValueProvider<SensorGroup, int>>();
            builder.Services.AddTransient<IQueryableProvider<SensorGroup>, QueryableProvider<SensorGroup>>();
            builder.Services.AddTransient<ISensorGroupRepository, SensorGroupRepository>();

            builder.Services.AddTransient<IKeyValueProvider<Unit, int>, KeyValueProvider<Unit, int>>();
            builder.Services.AddTransient<IQueryableProvider<Unit>, QueryableProvider<Unit>>();
            builder.Services.AddTransient<IUnitRepository, UnitRepository>();
            
            return builder;
        }
    }
}
