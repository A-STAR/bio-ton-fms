using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class SensorGroupMappingProfile : Profile
    {
        public SensorGroupMappingProfile()
        {
            CreateMap<SensorGroup, SensorGroupDto>();
        }
    }
}
