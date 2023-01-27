using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos;

namespace BioTonFMS.Telematica.Mapping
{
    public class SensorTypeMappingProfile : Profile
    {
        public SensorTypeMappingProfile()
        {
            CreateMap<SensorType, SensorTypeDto>()
                .ForMember(s => s.Unit,
                    opt => opt.MapFrom<ForeignKeyValue<int, string>?>(s =>
                        s.Unit == null ? null : new ForeignKeyValue<int, string>(s.Unit.Id, s.Unit.Name)))
                .ForMember(s => s.SensorGroup,
                    opt => opt.MapFrom(s => new ForeignKeyValue<int, string>(s.SensorGroupId, s.SensorGroup.Name)));
            CreateMap<SensorType, SensorTypeNestedDto>();
        }
    }
}
