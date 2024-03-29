﻿using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos.Vehicle;

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
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src =>
                    new ForeignKeyValue<int, string>(src.FuelType.Id, src.FuelType.Name)))
                .ForMember(dest => dest.VehicleGroup, opt => opt.Ignore())
                .ForMember(dest => dest.Tracker, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.VehicleGroupId.HasValue)
                    {
                        dest.VehicleGroup = 
                            new ForeignKeyValue<int, string>(src.VehicleGroupId.Value, src.VehicleGroup!.Name.ToString());
                    }
                    else
                    {
                        dest.VehicleGroup = null;
                    }
                    if (src.TrackerId.HasValue)
                    {
                        dest.Tracker = 
                            new ForeignKeyValue<int, string>(src.TrackerId.Value, src.Tracker!.Name.ToString());
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
