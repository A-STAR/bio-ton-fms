using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class VehicleMappingProfile : Profile
    {
        public VehicleMappingProfile()
        {
            CreateMap<VehiclesRequest, VehiclesFilter>();

            CreateMap<CreateVehicleDto, Vehicle>()
                .ForMember(dest => dest.VehicleSubType, opt => opt.MapFrom(src => src.SubType));

            CreateMap<UpdateVehicleDto, Vehicle>()
                .ForMember(dest => dest.VehicleSubType, opt => opt.MapFrom(src => src.SubType));

            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.VehicleGroupId, opt => opt.MapFrom(src => src.VehicleGroup.Id))
                .ForMember(dest => dest.SubType, opt => opt.MapFrom(src => src.VehicleSubType))
                .ForMember(dest => dest.FuelTypeId, opt => opt.MapFrom(src => src.FuelType.Id));

            CreateMap<Tracker, VehicleTrackerDto>();
        }
    }
}
