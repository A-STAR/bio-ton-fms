using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Dtos.Vehicle;

namespace BioTonFMS.Telematica.Mapping
{
    public class MonitoringMappingProfile : Profile
    {
        public MonitoringMappingProfile()
        {
            CreateMap<Vehicle, MonitoringVehicleDto>()
                .AfterMap((src, dest) =>
                {
                    if (src.TrackerId.HasValue)
                    {
                        dest.TrackerExternalId = src.Tracker!.ExternalId;
                        dest.TrackerImei = src.Tracker!.Imei;
                    }
                    else
                    {
                        dest.TrackerExternalId = null;
                        dest.TrackerImei = null;
                    }
                });

        }
    }
}
