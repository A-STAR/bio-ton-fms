using BioTonFMS.Telematica.Mapping;

namespace BioTonFMSApp.Startup
{
    public static class MappingProfileExtensions
    {
        public static WebApplicationBuilder AddMappingProfiles(this WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(TrackerMappingProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(VehicleMappingProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(VehicleGroupProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(FuelTypeMappingProfile).Assembly);
            return builder;
        }
    }
}
