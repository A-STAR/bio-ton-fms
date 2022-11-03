using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Dtos;
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
public class VehicleController : ControllerBase
{
    private readonly ILogger<VehicleController> _logger;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ITrackerRepository _trackerRepo;
    private readonly IMapper _mapper;

    public VehicleController(
        IVehicleRepository vehicleRepository,
        ITrackerRepository trackerRepo,
        IMapper mapper,
        ILogger<VehicleController> logger)
    {
        _trackerRepo = trackerRepo;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Возвращает список машин согласно заданному фильтру, условиям сортировки и параметрам постраничного отображения
    /// </summary>
    /// <param name="vehiclesRequest">Параметры запроса для списка машин</param>
    /// <response code="200">Список машин успешно возвращен</response>
    /// <response code="400">Невозможно вернуть список машин</response>
    [HttpGet("vehicles")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    public IActionResult GetVehicles([FromQuery] VehiclesRequest vehiclesRequest)
    {
        var filter = _mapper.Map<VehiclesFilter>(vehiclesRequest);

        var vehiclesPaging = _vehicleRepository.GetVehicles(filter);

        var result = new VehicleResponse
        {
            Vehicles = vehiclesPaging.Results.Select(res => _mapper.Map<VehicleDto>(res)).ToArray(),
            Pagination = new Pagination
            {
                PageIndex = vehiclesPaging.CurrentPage,
                Total = vehiclesPaging.TotalPageCount
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// Возвращает машину по Id
    /// </summary>
    /// <param name="id">Id машины</param>
    /// <response code="200">Машина успешно возвращена</response>
    /// <response code="404">Машина не найдена</response>
    [HttpGet("vehicle/{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult GetVehicle(int id)
    {
        var vehicle = _vehicleRepository[id];

        if (vehicle is not null)
        {
            var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
            return Ok(vehicleDto);
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Машина с id = {id} не найдена"));
        }
    }

    /// <summary>
    /// Добавляет машину
    /// </summary>
    /// <param name="createVehicleDto">Модель создания машины</param>
    /// <response code="200">Новая машина успешно создана</response>
    [HttpPost("vehicle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult AddVehicle(CreateVehicleDto createVehicleDto)
    {       
        var newVehicle = _mapper.Map<Vehicle>(createVehicleDto);

        if (createVehicleDto.TrackerId.HasValue)
        {
            var trackerId = createVehicleDto.TrackerId.Value;
            var tracker = _trackerRepo[trackerId];

            if (tracker is null)
            {
                return NotFound(
                    new ServiceErrorResult($"Трекер с id = {trackerId} не найден"));
            }
        }

        try
        {
            _vehicleRepository.Put(newVehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении машины {@newVehicle}", newVehicle);
            throw;
        }

        return Ok();
    }

    /// <summary>
    /// Обновляет машину
    /// </summary>
    /// <param name="id">Id машины</param>
    /// <param name="updateVehicleDto">Модель обновления машины</param>
    /// <response code="200">Машина успешно обновлена</response>
    /// <response code="404">Машина не найдена</response>
    [HttpPut("vehicle/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult UpdateVehicle(int id, UpdateVehicleDto updateVehicleDto)
    {
        var vehicle = _vehicleRepository[id];

        if (vehicle is not null)
        {
            if (updateVehicleDto.TrackerId.HasValue)
            {
                var trackerId = updateVehicleDto.TrackerId.Value;
                var tracker = _trackerRepo[trackerId];

                if (tracker is null)
                {
                    return NotFound(
                        new ServiceErrorResult($"Трекер с id = {trackerId} не найден"));
                }
            }
            _mapper.Map(updateVehicleDto, vehicle);

            try
            {
                _vehicleRepository.Update(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении машины {@id}", vehicle.Id);
                throw;
            }

            return Ok();
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Машина с id = {id} не найдена"));
        }
    }

    /// <summary>
    /// Удаляет машину
    /// </summary>
    /// <param name="id">Id машины</param>
    /// <response code="200">Машина успешно удалена</response>
    /// <response code="404">Машина не найдена</response>
    [HttpDelete("vehicle/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteVehicle(int id)
    {
        var vehicle = _vehicleRepository[id];

        if (vehicle is not null)
        {
            try
            {
                _vehicleRepository.Remove(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при машины {@id}", vehicle.Id);
                throw;
            }

            return Ok();
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Трекер с id = {id} не найден"));
        }
    }
}