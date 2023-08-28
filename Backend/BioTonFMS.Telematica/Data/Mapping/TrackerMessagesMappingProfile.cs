using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos.Tracker;

namespace BioTonFMS.Telematica.Mapping
{
    public class TrackerMessagesMappingProfile : Profile
    {
        public TrackerMessagesMappingProfile()
        {
            CreateMap<TrackerMessageCsv, TrackerMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ServerDateTime,
                    opt => opt.MapFrom(src => src.ServerDateTime.ToUniversalTime()))
                .ForMember(dest => dest.TrackerDateTime,
                    opt => opt.MapFrom(src => GetDateTime(src.TrackerDateTime)))
                .ForMember(dest => dest.PackageUID, opt => opt.Ignore());
        }

        public DateTime? GetDateTime(DateTime? trackerDateTime)
        {
            if (trackerDateTime == null)
            {
                return null;
            }
            else
            {
                return trackerDateTime.Value.ToUniversalTime();
            }
        }
    }
}
