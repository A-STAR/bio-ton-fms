using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos;
using KeyValuePair = BioTonFMS.Infrastructure.Extensions.KeyValuePair;

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
                .ForMember(dest => dest.SubType, opt => opt.MapFrom(src =>
                    EnumExtension.GetKeyValuePair(src.VehicleSubType)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    EnumExtension.GetKeyValuePair(src.Type)))
                .ForMember(dest => dest.VehicleGroup, opt => opt.MapFrom(src =>
                    new KeyValuePair()
                    {
                        Key = src.VehicleGroup.Id.ToString(),
                        Value = src.VehicleGroup.Name
                    }))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src =>
                    new KeyValuePair()
                    {
                        Key = src.FuelType.Id.ToString(),
                        Value = src.FuelType.Name
                    }))
                .ForMember(dest => dest.Tracker, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.TrackerId.HasValue)
                    {
                        dest.Tracker = new KeyValuePair()
                        {
                            Key = src.TrackerId.Value.ToString(),
                            Value = src.Tracker!.Name.ToString()
                        };
                    }
                    else
                    {
                        dest.Tracker = null;
                    }
                });

            CreateMap<Tracker, VehicleTrackerDto>();
        }
    }
}
