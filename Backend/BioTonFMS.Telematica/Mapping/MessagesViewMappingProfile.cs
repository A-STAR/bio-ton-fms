using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Telematica.Dtos.MessagesView;

namespace BioTonFMS.Telematica.Mapping
{
    public class MessagesViewMappingProfile : Profile
    {
        public MessagesViewMappingProfile()
        {
            CreateMap<Vehicle, MessagesViewVehicleDto>();
        }
    }
}
