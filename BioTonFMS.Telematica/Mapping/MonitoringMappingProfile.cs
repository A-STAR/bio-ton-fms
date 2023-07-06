using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Dtos.Vehicle;

namespace BioTonFMS.Telematica.Mapping
{
    public class MonitoringMappingProfile : Profile
    {
        public MonitoringMappingProfile()
        {
            CreateMap<Vehicle, MonitoringVehicleDto>()
                .ForMember(dest => dest.SubType, opt => opt.MapFrom(src =>
                    EnumExtension.GetKeyValuePair(src.VehicleSubType)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    EnumExtension.GetKeyValuePair(src.Type)))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src =>
                    new ForeignKeyValue<int, string>(src.FuelType.Id, src.FuelType.Name)))
                .ForMember(dest => dest.VehicleGroup, opt => opt.Ignore())
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
                });
        }
    }
}
