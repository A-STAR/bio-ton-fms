using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos.Tracker;
using KeyValuePair = BioTonFMS.Infrastructure.Extensions.KeyValuePair;

namespace BioTonFMS.Telematica.Mapping
{
    public class TrackerMappingProfile : Profile
    {
        public TrackerMappingProfile()
        {
            CreateMap<TrackersRequest, TrackersFilter>();
            CreateMap<CreateTrackerDto, Tracker>();
            CreateMap<UpdateTrackerDto, Tracker>();
            CreateMap<Tracker, TrackerDto>()
                .ForMember(dest => dest.TrackerType, opt => opt.MapFrom(src =>
                    EnumExtension.GetKeyValuePair(src.TrackerType)))
                .ForMember(dest => dest.Vehicle,
                    opt =>
                    {
                        opt.PreCondition(src => src.Vehicle != null);
                        opt.MapFrom(src => new ForeignKeyValue<int, string>(src.Vehicle!.Id, src.Vehicle.Name));
                    });
        }
    }
}
