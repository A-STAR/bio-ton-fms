using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class SensorTypeMappingProfile : Profile
    {
        public SensorTypeMappingProfile()
        {
            CreateMap<SensorType, SensorTypeDto>();
        }
    }
}
