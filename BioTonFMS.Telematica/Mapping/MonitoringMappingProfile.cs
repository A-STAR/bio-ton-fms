﻿using AutoMapper;
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
            CreateMap<Vehicle, MonitoringVehicleDto>();
            CreateMap<Tracker, MonitoringTrackerDto>();
        }
    }
}