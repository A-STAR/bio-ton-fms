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
            CreateMap<SensorsRequest, SensorsFilter>();
            CreateMap<CreateSensorDto, Sensor>();
            CreateMap<UpdateSensorDto, Sensor>();
            CreateMap<Sensor, SensorDto>()
                .ForMember(dst => dst.Tracker, opt =>
                    opt.MapFrom(s => new ForeignKeyValue<int, string>(s.TrackerId, s.Tracker.Name)))
                .ForMember(dst => dst.SensorType, opt =>
                    opt.MapFrom(s => new ForeignKeyValue<int, string>(s.SensorTypeId, s.SensorType.Name)))
                .ForMember(dst => dst.Unit, opt =>
                    opt.MapFrom(s => new ForeignKeyValue<int, string>(s.UnitId, s.Unit.Name)))
                .ForMember(dst => dst.Validator, opt =>
                    opt.MapFrom<ForeignKeyValue<int, string>?>(s =>
                        s.Validator == null ? null : new ForeignKeyValue<int, string>(s.Validator.Id,  s.Validator.Name)));
        }
    }
}
