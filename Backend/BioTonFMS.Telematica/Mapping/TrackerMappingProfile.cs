﻿using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos.Tracker;

namespace BioTonFMS.Telematica.Mapping
{
    public class TrackerMappingProfile : Profile
    {
        public TrackerMappingProfile()
        {
            CreateMap<TrackersRequest, TrackersFilter>();
            CreateMap<CreateTrackerDto, Tracker>()
                .ForMember(dest => dest.StartDate,
                    opt => opt.MapFrom(src => (src.StartDate ?? DateTime.UtcNow).ToUniversalTime()));
            CreateMap<UpdateTrackerDto, Tracker>()
                .ForMember(dest => dest.StartDate,
                    opt => opt.MapFrom(src => src.StartDate.ToUniversalTime()));
            CreateMap<Tracker, TrackerDto>()
                .ForMember(dest => dest.TrackerType, opt => opt.MapFrom(src =>
                    EnumExtension.GetKeyValuePair(src.TrackerType)))
                .ForMember(dest => dest.Vehicle,
                    opt =>
                    {
                        opt.PreCondition(src => src.Vehicle is not null);
                        opt.MapFrom(src => new ForeignKeyValue<int, string>(src.Vehicle!.Id, src.Vehicle.Name));
                    });
        }
    }
}
