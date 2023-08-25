using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class FuelTypeMappingProfile : Profile
    {
        public FuelTypeMappingProfile()
        {
            CreateMap<FuelType, FuelTypeDto>();
        }
    }
}
