using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Dtos;
using FluentValidation;
using FluentValidation.Results;
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
public class VehicleController : ValidationControllerBase
{
    private readonly IVehicleRepository _vehicleRepo;
    private readonly ITrackerRepository _trackerRepo;
    private readonly IFuelTypeRepository _fuelTypeRepo;
    private readonly IVehicleGroupRepository _vehicleGroupRepo;
    
    private readonly IValidator<UpdateVehicleDto> _updateValidator;
    private readonly IValidator<VehiclesRequest> _vehiclesRequestValidator;
    private readonly IValidator<CreateVehicleDto> _createValidator;

    private readonly ILogger<VehicleController> _logger;
    private readonly IMapper _mapper;

    public VehicleController(
        IMapper mapper,
        ILogger<VehicleController> logger,
        IVehicleRepository vehicleRepo,
        ITrackerRepository trackerRepo,
        IFuelTypeRepository fuelTypeRepo,
        IVehicleGroupRepository vehicleGroupRepo,
        IValidator<CreateVehicleDto> createValidator,
        IValidator<UpdateVehicleDto> updateValidator,
        IValidator<VehiclesRequest> vehiclesRequestValidator)
    {
        _updateValidator = updateValidator;
        _createValidator = createValidator;
        _vehiclesRequestValidator = vehiclesRequestValidator;
        _trackerRepo = trackerRepo;
        _fuelTypeRepo = fuelTypeRepo;
        _vehicleGroupRepo = vehicleGroupRepo;
        _vehicleRepo = vehicleRepo;
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
        ValidationResult validationResult = _vehiclesRequestValidator
            .Validate(vehiclesRequest);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var filter = _mapper.Map<VehiclesFilter>(vehiclesRequest);
        var vehiclesPaging = _vehicleRepo.GetVehicles(filter);
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
    [HttpGet("vehicle/{id:int}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult GetVehicle(int id)
    {
        var vehicle = _vehicleRepo[id];

        if (vehicle is null)
        {
            return NotFound(new ServiceErrorResult($"Машина с id = {id} не найдена"));
        }
        var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
        return Ok(vehicleDto);
    }

    /// <summary>
    /// Добавляет машину
    /// </summary>
    /// <param name="createVehicleDto">Модель создания машины</param>
    /// <response code="200">Новая машина успешно создана</response>
    [HttpPost("vehicle")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status409Conflict)]
    public IActionResult AddVehicle(CreateVehicleDto createVehicleDto)
    {
        ValidationResult validationResult = _createValidator.Validate(createVehicleDto);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        if (_fuelTypeRepo[createVehicleDto.FuelTypeId] is null)
        {
            return NotFound(
                new ServiceErrorResult($"Тип топлива с id = {createVehicleDto.FuelTypeId} не найден"));
        }
        if (createVehicleDto.VehicleGroupId.HasValue && _vehicleGroupRepo[createVehicleDto.VehicleGroupId.Value] is null)
        {
            return NotFound(
                new ServiceErrorResult($"Группа машин с id = {createVehicleDto.VehicleGroupId.Value} не найдена"));
        }
        if (createVehicleDto.TrackerId.HasValue && _trackerRepo[createVehicleDto.TrackerId.Value] is null)
        {
            return NotFound(
                new ServiceErrorResult($"Трекер с id = {createVehicleDto.TrackerId.Value} не найден"));
        }

        var newVehicle = _mapper.Map<Vehicle>(createVehicleDto);
        try
        {
            _vehicleRepo.Add(newVehicle);
            var vehicleDto = _mapper.Map<VehicleDto>(newVehicle);
            return Ok(vehicleDto);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new ServiceErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении машины {@newVehicle}", newVehicle);
            throw;
        }
    }

    /// <summary>
    /// Обновляет машину
    /// </summary>
    /// <param name="id">Id машины</param>
    /// <param name="updateVehicleDto">Модель обновления машины</param>
    /// <response code="200">Машина успешно обновлена</response>
    /// <response code="404">Машина не найдена</response>
    [HttpPut("vehicle/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status409Conflict)]
    public IActionResult UpdateVehicle(int id, UpdateVehicleDto updateVehicleDto)
    {
        ValidationResult validationResult = _updateValidator.Validate(updateVehicleDto);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var vehicle = _vehicleRepo[id];
        if (vehicle is null)
        {
            return NotFound(new ServiceErrorResult($"Машина с id = {id} не найдена"));
        }
        if (_fuelTypeRepo[updateVehicleDto.FuelTypeId] is null)
        {
            return NotFound(
                new ServiceErrorResult($"Тип топлива с id = {updateVehicleDto.FuelTypeId} не найден"));
        }
        if (updateVehicleDto.VehicleGroupId.HasValue && _vehicleGroupRepo[updateVehicleDto.VehicleGroupId.Value] is null)
        {
            return NotFound(
                new ServiceErrorResult($"Группа машин с id = {updateVehicleDto.VehicleGroupId.Value} не найдена"));
        }
        if (updateVehicleDto.TrackerId.HasValue && _trackerRepo[updateVehicleDto.TrackerId.Value] is null)
        {
            return NotFound(
                new ServiceErrorResult($"Трекер с id = {updateVehicleDto.TrackerId.Value} не найден"));
        }

        _mapper.Map(updateVehicleDto, vehicle);
        
        try
        {
            _vehicleRepo.Update(vehicle);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new ServiceErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении машины {@id}", vehicle.Id);
            throw;
        }
        
        return Ok();
    }

    /// <summary>
    /// Удаляет машину
    /// </summary>
    /// <param name="id">Id машины</param>
    /// <response code="200">Машина успешно удалена</response>
    /// <response code="404">Машина не найдена</response>
    [HttpDelete("vehicle/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult DeleteVehicle(int id)
    {
        var vehicle = _vehicleRepo[id];
        if (vehicle is null)
        {
            return NotFound(new ServiceErrorResult($"Машина с id = {id} не найдена"));
        }
        
        try
        {
            _vehicleRepo.Remove(vehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при машины {@id}", vehicle.Id);
            throw;
        }
        return Ok();
    }
}