using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.EF.Providers;
using BioTonFMS.Infrastructure.EF.Repositories;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;

namespace BioTonFMSApp.Startup
{
    public static class DependencyRegistrationExtensions
    {
        public static WebApplicationBuilder RegisterInfrastructureComponents(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<Factory<DbContext>>(sp =>
            {
                var instance = sp.GetRequiredService<BioTonDBContext>();
                return () => instance;
            });

            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

            builder.Services.AddTransient(sp =>
            {
                Console.WriteLine("UnitOfWorkFactory invoked");
                return new UnitOfWorkFactory(sp);
            });
            return builder;
        }

        public static WebApplicationBuilder RegisterDataAccess(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IKeyValueProvider<Tracker, int>, KeyValueProvider<Tracker, int>>();
            builder.Services.AddTransient<IQueryableProvider<Tracker>, QueryableProvider<Tracker>>();
            builder.Services.AddTransient<ITrackerRepository, TrackerRepository>();

            builder.Services.AddTransient<IKeyValueProvider<Vehicle, int>, KeyValueProvider<Vehicle, int>>();
            builder.Services.AddTransient<IQueryableProvider<Vehicle>, QueryableProvider<Vehicle>>();
            builder.Services.AddTransient<IVehicleRepository, VehicleRepository>();

            builder.Services.AddTransient<IKeyValueProvider<VehicleGroup, int>, KeyValueProvider<VehicleGroup, int>>();
            builder.Services.AddTransient<IQueryableProvider<VehicleGroup>, QueryableProvider<VehicleGroup>>();
            builder.Services.AddTransient<IVehicleGroupRepository, VehicleGroupRepository>();

            builder.Services.AddTransient<IKeyValueProvider<FuelType, int>, KeyValueProvider<FuelType, int>>();
            builder.Services.AddTransient<IQueryableProvider<FuelType>, QueryableProvider<FuelType>>();
            builder.Services.AddTransient<IFuelTypeRepository, FuelTypeRepository>();

            return builder;
        }
    }
}
