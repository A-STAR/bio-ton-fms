using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class TrackerMappingProfile : Profile
    {
        public TrackerMappingProfile()
        {
            CreateMap<TrackersRequest, TrackersFilter>();
            CreateMap<CreateTrackerDto, Tracker>();
            CreateMap<UpdateTrackerDto, Tracker>(); 
            CreateMap<Tracker, TrackerDto>();
        }
    }
}
