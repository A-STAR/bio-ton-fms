using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Dtos;
using BioTonFMS.Telematica.Expressions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// Сервис для работы с датчиками
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica")]
[Consumes("application/json")]
[Produces("application/json")]
public class SensorController : ValidationControllerBase
{
    private readonly ISensorRepository _sensorRepository;
    private readonly ITrackerRepository _trackerRepository;
    private readonly ITrackerTagRepository _trackerTagRepository;
    private readonly ISensorTypeRepository _sensorTypeRepository;
    private readonly IValidator<UpdateSensorDto> _updateValidator;
    private readonly IValidator<SensorsRequest> _sensorsRequestValidator;
    private readonly IValidator<CreateSensorDto> _createValidator;
    private readonly IValidator<Sensor> _sensorValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<SensorController> _logger;

    public SensorController(
        ISensorRepository sensorRepository,
        ISensorTypeRepository sensorTypeRepository,
        IMapper mapper,
        IValidator<UpdateSensorDto> updateValidator,
        IValidator<CreateSensorDto> createValidator,
        IValidator<Sensor> sensorValidator,
        IValidator<SensorsRequest> sensorsRequestValidator,
        ITrackerRepository trackerRepository, ILogger<SensorController> logger, ITrackerTagRepository trackerTagRepository)
    {
        _sensorRepository = sensorRepository;
        _sensorTypeRepository = sensorTypeRepository;
        _mapper = mapper;
        _updateValidator = updateValidator;
        _createValidator = createValidator;
        _sensorValidator = sensorValidator;
        _sensorsRequestValidator = sensorsRequestValidator;
        _trackerRepository = trackerRepository;
        _logger = logger;
        _trackerTagRepository = trackerTagRepository;
    }

    /// <summary>
    /// Возвращает список датчиков согласно заданному фильтру, условиям сортировки и параметрам постраничного отображения
    /// </summary>
    /// <param name="sensorsRequest">Параметры запроса для списка датчиков</param>
    /// <response code="200">Список датчиков успешно возвращен</response>
    /// <response code="400">Невозможно вернуть список датчиков</response>
    [HttpGet("sensors")]
    [ProducesResponseType(typeof( SensorsResponse ), StatusCodes.Status200OK)]
    public IActionResult GetSensors([FromQuery] SensorsRequest sensorsRequest)
    {
        var validationResult = _sensorsRequestValidator
            .Validate(sensorsRequest);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var filter = _mapper.Map<SensorsFilter>(sensorsRequest);

        var sensorsPaging = _sensorRepository.GetSensors(filter);

        var result = new SensorsResponse
        {
            Sensors = sensorsPaging.Results.Select(res => _mapper.Map<SensorDto>(res)).ToArray(), Pagination = new Pagination
            {
                PageIndex = sensorsPaging.CurrentPage, Total = sensorsPaging.TotalPageCount
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// Возвращает датчик  по Id
    /// </summary>
    /// <param name="id">Id датчика</param>
    /// <response code="200">Датчик успешно возвращен</response>
    /// <response code="404">Датчик не найден</response>
    [HttpGet("sensor/{id:int}")]
    [ProducesResponseType(typeof( SensorDto ), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof( ServiceErrorResult ), StatusCodes.Status404NotFound)]
    public IActionResult GetSensor(int id)
    {
        var sensor = _sensorRepository[id];
        if (sensor is not null)
        {
            var sensorDto = _mapper.Map<SensorDto>(sensor);
            return Ok(sensorDto);
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Датчик с id = {id} не найден"));
        }
    }

    /// <summary>
    /// Добавляет датчик
    /// </summary>
    /// <param name="createSensorDto">Модель создания датчика</param>
    /// <response code="200">Новый датчик успешно создан, возвращает данные нового датчика</response>
    /// <response code="409">Конфликт при обновлении датчика</response>
    [HttpPost("sensor")]
    [ProducesResponseType(typeof( SensorDto ), StatusCodes.Status200OK)]
    public IActionResult AddSensor(CreateSensorDto createSensorDto)
    {
        var validationResult1 = _createValidator.Validate(createSensorDto);
        if (!validationResult1.IsValid)
        {
            return ReturnValidationErrors(validationResult1);
        }

        var newSensor = _mapper.Map<Sensor>(createSensorDto);

        ApplySensorTypeConstraints(newSensor);
        
        var tracker = _trackerRepository[newSensor.TrackerId];
        if (tracker is null)
        {
            return BadRequest($"Трекер с идентификатором {newSensor.TrackerId} не существует!");
        }
        tracker.Sensors.Add(newSensor);

        var trackerTags = _trackerTagRepository.GetTags();

        var validationResults = tracker.ValidateSensor(trackerTags, newSensor, _logger, _sensorValidator);

        var hasValidationErrors = ProcessValidationResults(validationResults);
        if (hasValidationErrors)
            return ReturnValidationErrors();

        try
        {
            _sensorRepository.Add(newSensor);
            var sensorDto = _mapper.Map<SensorDto>(newSensor);
            return Ok(sensorDto);
        }
        catch( ArgumentException ex )
        {
            return Conflict(new ServiceErrorResult(ex.Message));
        }
    }
    private bool ProcessValidationResults(IEnumerable<SensorProblemDescription> validationResults)
    {

        var hasValidationErrors = false;
        foreach (var problem in validationResults)
        {
            switch (problem.FieldName)
            {
                case "error":
                    _logger.LogError("{}", problem.Message);
                    break;
                case "warning":
                    _logger.LogWarning("{}", problem.Message);
                    break;
                default:
                    ModelState.AddModelError(problem.FieldName, problem.Message);
                    hasValidationErrors = true;
                    break;
            }
        }
        return hasValidationErrors;
    }

    /// <summary>
    /// Обновляет датчик
    /// </summary>
    /// <param name="id">Id датчика</param>
    /// <param name="updateSensorDto">Модель обновления датчика</param>
    /// <response code="200">Датчик успешно обновлен</response>
    /// <response code="404">Датчик не найден</response>
    /// <response code="409">Конфликт при обновлении датчика</response>
    [HttpPut("sensor/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof( ServiceErrorResult ), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof( ServiceErrorResult ), StatusCodes.Status409Conflict)]
    public IActionResult UpdateSensor(int id, UpdateSensorDto updateSensorDto)
    {
        var validationResult = _updateValidator.Validate(updateSensorDto);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var updatedSensor = _sensorRepository[id];
        if (updatedSensor is not null)
        {
            _mapper.Map(updateSensorDto, updatedSensor);

            ApplySensorTypeConstraints(updatedSensor);
            
            var tracker = _trackerRepository[updatedSensor.TrackerId];
            if (tracker is null)
            {
                return BadRequest($"Трекер с идентификатором {updatedSensor.TrackerId} не существует!");
            }
            tracker.Sensors.RemoveAll(s => s.Id == id);
            tracker.Sensors.Add(updatedSensor);

            var trackerTags = _trackerTagRepository.GetTags();

            var validationResults = tracker.ValidateSensor(trackerTags, updatedSensor, _logger, _sensorValidator);

            var hasValidationErrors = ProcessValidationResults(validationResults);
            if (hasValidationErrors)
                return ReturnValidationErrors();

            try
            {
                _sensorRepository.Update(updatedSensor);
            }
            catch( ArgumentException ex )
            {
                return Conflict(new ServiceErrorResult(ex.Message));
            }
            return Ok();
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Датчик с id = {id} не найден"));
        }
    }

    /// <summary>
    /// Удаляет датчик
    /// </summary>
    /// <param name="id">Id датчика</param>
    /// <response code="200">Датчик успешно удален</response>
    /// <response code="404">Датчик не найден</response>
    /// <response code="409">Конфликт при удалении датчика</response>
    [HttpDelete("sensor/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult DeleteSensor(int id)
    {
        var sensorToDelete = _sensorRepository[id];

        if (sensorToDelete is null)
        {
            return NotFound(new ServiceErrorResult($"Датчик с id = {id} не найден"));            
        }
        
        var tracker = _trackerRepository[sensorToDelete.TrackerId];
        if (tracker is null)
        {
            return Conflict($"Трекер с идентификатором {sensorToDelete.TrackerId} не существует");
        }

        var validationResult = tracker.ValidateSensorRemoval(sensorToDelete, _logger);
        if (!string.IsNullOrEmpty(validationResult))
        {
            return Conflict(validationResult);
        }
        
        try
        {
            _sensorRepository.Remove(sensorToDelete);
            return Ok();
        }
        catch( ArgumentException ex )
        {
            return Conflict(new ServiceErrorResult(ex.Message));
        }
    }

    private void ApplySensorTypeConstraints(Sensor newSensor)
    {
        var sensorType = _sensorTypeRepository[newSensor.SensorTypeId];
        if (sensorType is null)
            return;
        if (sensorType.UnitId.HasValue)
            newSensor.UnitId = sensorType.UnitId.Value;
        if (sensorType.DataType.HasValue)
            newSensor.DataType = sensorType.DataType.Value;
    }
}
