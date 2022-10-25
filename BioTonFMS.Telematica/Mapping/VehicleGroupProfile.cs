using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class VehicleGroupProfile : Profile
    {
        public VehicleGroupProfile()
        {
            CreateMap<VehicleGroup, VehicleGroupDto>();
        }
    }
}
