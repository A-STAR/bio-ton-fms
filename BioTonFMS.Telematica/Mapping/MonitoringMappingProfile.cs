using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Telematica.Dtos.Monitoring;

namespace BioTonFMS.Telematica.Mapping
{
    public class MonitoringMappingProfile : Profile
    {
        public MonitoringMappingProfile()
        {
            CreateMap<Vehicle, MonitoringVehicleDto>();
            CreateMap<Tracker, MonitoringTrackerDto>();

            CreateMap<Tracker, MonitoringTrackerInfoDto>()
                .ForMember(dst => dst.TrackerType, opt => opt.Ignore())
                .ForMember(dst => dst.Sensors, opt => opt.Ignore())
                .ForMember(dst => dst.Parameters, opt => opt.Ignore());

            CreateMap<TrackerMessage, MonitoringGeneralInfoDto>()
                .ForMember(dst => dst.LastMessageTime, opt => opt.MapFrom(src => src.TrackerDateTime))
                .ForMember(dst => dst.SatellitesNumber, opt => opt.MapFrom(src => src.SatNumber))
                .ForMember(dst => dst.EngineHours, opt => opt.Ignore())
                .ForMember(dst => dst.Mileage, opt => opt.Ignore());
        }
    }
}
