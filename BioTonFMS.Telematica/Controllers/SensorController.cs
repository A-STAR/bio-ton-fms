using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    private readonly ISensorRepository _sensorRepo;
    private readonly IValidator<UpdateSensorDto> _updateValidator;
    private readonly IValidator<SensorsRequest> _sensorsRequestValidator;
    private readonly IValidator<CreateSensorDto> _createValidator;
    private readonly IMapper _mapper;

    public SensorController(
        ISensorRepository sensorRepo,
        IMapper mapper,
        IValidator<UpdateSensorDto> updateValidator,
        IValidator<CreateSensorDto> createValidator,
        IValidator<SensorsRequest> sensorsRequestValidator)
    {
        _sensorRepo = sensorRepo;
        _mapper = mapper;
        _updateValidator = updateValidator;
        _createValidator = createValidator;
        _sensorsRequestValidator = sensorsRequestValidator;
    }

    /// <summary>
    /// Возвращает список датчиков согласно заданному фильтру, условиям сортировки и параметрам постраничного отображения
    /// </summary>
    /// <param name="sensorsRequest">Параметры запроса для списка датчиков</param>
    /// <response code="200">Список датчиков успешно возвращен</response>
    /// <response code="400">Невозможно вернуть список датчиков</response>
    [HttpGet("sensors")]
    [ProducesResponseType(typeof(SensorsResponse), StatusCodes.Status200OK)]
    public IActionResult GetSensors([FromQuery] SensorsRequest sensorsRequest)
    {
        var validationResult = _sensorsRequestValidator
            .Validate(sensorsRequest);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var filter = _mapper.Map<SensorsFilter>(sensorsRequest);

        var sensorsPaging = _sensorRepo.GetSensors(filter);

        var result = new SensorsResponse
        {
            Sensors = sensorsPaging.Results.Select(res => _mapper.Map<SensorDto>(res)).ToArray(),
            Pagination = new Pagination
            {
                PageIndex = sensorsPaging.CurrentPage,
                Total = sensorsPaging.TotalPageCount
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
    [ProducesResponseType(typeof(SensorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult GetSensor(int id)
    {
        var sensor = _sensorRepo[id];
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
    [ProducesResponseType(typeof(SensorDto), StatusCodes.Status200OK)]
    public IActionResult AddSensor(CreateSensorDto createSensorDto)
    {
        var validationResult = _createValidator.Validate(createSensorDto);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var newSensor = _mapper.Map<Sensor>(createSensorDto);
        try
        {
            _sensorRepo.Add(newSensor);
            var sensorDto = _mapper.Map<SensorDto>(newSensor);
            return Ok(sensorDto);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new ServiceErrorResult(ex.Message));
        }
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
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status409Conflict)]
    public IActionResult UpdateSensor(int id, UpdateSensorDto updateSensorDto)
    {
        var validationResult = _updateValidator.Validate(updateSensorDto);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var sensor = _sensorRepo[id];
        if (sensor is not null)
        {
            _mapper.Map(updateSensorDto, sensor);

            try
            {
                _sensorRepo.Update(sensor);
            }
            catch (ArgumentException ex)
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
        var sensor = _sensorRepo[id];
        if (sensor is not null)
        {
            try
            {
                _sensorRepo.Remove(sensor);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return Conflict(new ServiceErrorResult(ex.Message));
            }
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Датчик с id = {id} не найден"));
        }
    }
}