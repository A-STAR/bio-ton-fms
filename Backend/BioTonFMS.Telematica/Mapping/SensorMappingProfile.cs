using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class SensorMappingProfile : Profile
    {
        public SensorMappingProfile()
        {
            CreateMap<Sensor, Sensor>();
            CreateMap<SensorsRequest, SensorsFilter>();
            CreateMap<CreateSensorDto, Sensor>()
                .ForMember(dst => dst.IsVisible, opt => opt.MapFrom(s => s.Visibility));
            CreateMap<UpdateSensorDto, Sensor>()
                .ForMember(dst => dst.IsVisible, opt => opt.MapFrom(s => s.Visibility));
            CreateMap<Sensor, SensorDto>()
                .ForMember(dst => dst.Visibility, opt => opt.MapFrom(s => s.IsVisible))
                .ForMember(dst => dst.Tracker, opt =>
                    opt.MapFrom(s => new ForeignKeyValue<int, string>(s.TrackerId, s.Tracker.Name)))
                .ForMember(dst => dst.SensorType, opt =>
                    opt.MapFrom(s => new ForeignKeyValue<int, string>(s.SensorTypeId, s.SensorType.Name)))
                .ForMember(dst => dst.Unit, opt =>
                    opt.MapFrom(s => new ForeignKeyValue<int, string>(s.UnitId, s.Unit.Name)))
                .ForMember(dst => dst.Validator, opt =>
                    opt.MapFrom<ForeignKeyValue<int, string>?>(s =>
                        s.Validator == null ? null : new ForeignKeyValue<int, string>(s.Validator.Id, s.Validator.Name)));
        }
    }
}
