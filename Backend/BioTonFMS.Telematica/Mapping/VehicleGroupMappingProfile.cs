using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class VehicleGroupMappingProfile : Profile
    {
        public VehicleGroupMappingProfile()
        {
            CreateMap<VehicleGroup, VehicleGroupDto>();
        }
    }
}
