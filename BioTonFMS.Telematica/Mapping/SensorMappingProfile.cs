using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
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
            CreateMap<Sensor, SensorDto>();
        }
    }
}
