using BioTonFMS.Telematica.Mapping;

namespace BioTonFMSApp.Startup
{
    public static class MappingProfileExtensions
    {
        public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(TrackerMappingProfile).Assembly);
            services.AddAutoMapper(typeof(VehicleMappingProfile).Assembly);
            services.AddAutoMapper(typeof(VehicleGroupMappingProfile).Assembly);
            services.AddAutoMapper(typeof(FuelTypeMappingProfile).Assembly);
            services.AddAutoMapper(typeof(MonitoringMappingProfile).Assembly);
            services.AddAutoMapper(typeof(MessagesViewMappingProfile).Assembly);
            return services;
        }
    }
}
