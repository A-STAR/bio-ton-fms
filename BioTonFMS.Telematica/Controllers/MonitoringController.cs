using AutoMapper;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Dtos.Vehicle;
using BioTonFMS.Telematica.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// API cервиса для работы с машинами
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica")]
[Consumes("application/json")]
[Produces("application/json")]
public class MonitoringController : ValidationControllerBase
{
    public MonitoringController(
        IMapper mapper,
        ILogger<MonitoringController> logger,
        IVehicleRepository vehicleRepository,
        ITrackerRepository trackerRepo)
    {
        _vehicleRepo = vehicleRepository;
    }

    /// <summary>
    /// Возвращает список машин согласно заданному критерию поиска
    /// </summary>
    /// <param name="findCriterion">Критерий поиска для списка машин</param>
    /// <response code="200">Список машин успешно возвращен</response>
    /// <response code="400">Невозможно вернуть список машин</response>
    [HttpGet("vehicles")]
    [ProducesResponseType(typeof(MonitoringVehicleDto[]), StatusCodes.Status200OK)]
    public IActionResult GetVehicles([FromQuery] string findCriterion)
    {
        //var filter = _mapper.Map<VehiclesFilter>(vehiclesRequest);
        var vehiclesPaging = _vehicleRepo.GetVehicles(filter);
        var result = vehiclesPaging.Results.Select(res => _mapper.Map<MonitoringVehicleDto>(res)).ToArray();

        return Ok(result);
    }

}
