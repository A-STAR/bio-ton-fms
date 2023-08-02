using AutoMapper;
using BioTonFMS.Common.Settings;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos.Monitoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// API cервиса для работы с машинами
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica/monitoring")]
[Consumes("application/json")]
[Produces("application/json")]
public class MonitoringController : ValidationControllerBase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly TrackerOptions _trackerOptions;

    public MonitoringController(
        IMapper mapper,
        ILogger<MonitoringController> logger,
        IVehicleRepository vehicleRepository,
        ITrackerMessageRepository messageRepository,
        IOptions<TrackerOptions> trackerOptions)
    {
        _vehicleRepository = vehicleRepository;
        _messageRepository = messageRepository;
        _trackerOptions = trackerOptions.Value;
        _mapper = mapper;
    }

    /// <summary>
    /// Возвращает список машин согласно заданному критерию поиска
    /// </summary>
    /// <param name="findCriterion">Критерий поиска для списка машин</param>
    /// <response code="200">Список машин успешно возвращен</response>
    /// <response code="400">Невозможно вернуть список машин</response>
    [HttpGet("vehicles")]
    [ProducesResponseType(typeof(MonitoringVehicleDto[]), StatusCodes.Status200OK)]
    public IActionResult FindVehicles([FromQuery] string? findCriterion)
    {
        Vehicle[] vehicles = _vehicleRepository.FindVehicles(findCriterion);

        MonitoringVehicleDto[] monitoringDtos = vehicles.Select(v => _mapper.Map<MonitoringVehicleDto>(v)).ToArray();

        int[] trackerExternalIds = monitoringDtos.Where(x => x.Tracker?.ExternalId != null)
            .Select(x => x.Tracker!.ExternalId!.Value).ToArray();
        
        var states = _messageRepository.GetVehicleStates(trackerExternalIds,
            _trackerOptions.TrackerAddressValidMinutes);
        
        foreach (MonitoringVehicleDto monitoringDto in monitoringDtos)
        {
            if (monitoringDto.Tracker?.ExternalId == null) continue;
            
            var status = states[monitoringDto.Tracker.ExternalId.Value];

            monitoringDto.MovementStatus = status.MovementStatus;
            monitoringDto.ConnectionStatus = status.ConnectionStatus;
            monitoringDto.LastMessageTime = status.LastMessageTime;
            monitoringDto.NumberOfSatellites = status.NumberOfSatellites;
        }

        return Ok(monitoringDtos);
    }
}
