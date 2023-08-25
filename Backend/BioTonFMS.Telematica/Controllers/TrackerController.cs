using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Dtos;
using BioTonFMS.Telematica.Dtos.Tracker;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// API cервиса для работы с трекерами
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica")]
[Consumes("application/json")]
[Produces("application/json")]
public class TrackerController : ValidationControllerBase
{
    private readonly ILogger<TrackerController> _logger;
    private readonly ITrackerRepository _trackerRepository;
    private readonly IValidator<UpdateTrackerDto> _updateValidator;
    private readonly IValidator<TrackersRequest> _trackersRequestValidator;
    private readonly IValidator<CreateTrackerDto> _createValidator;
    private readonly IMapper _mapper;

    public TrackerController(
        ITrackerRepository trackerRepository,
        IMapper mapper,
        ILogger<TrackerController> logger,
        IValidator<UpdateTrackerDto> updateValidator,
        IValidator<CreateTrackerDto> createValidator,
        IValidator<TrackersRequest> trackersRequestValidator)
    {
        _trackerRepository = trackerRepository;
        _mapper = mapper;
        _logger = logger;
        _updateValidator = updateValidator;
        _createValidator = createValidator;
        _trackersRequestValidator = trackersRequestValidator;
    }

    /// <summary>
    /// Возвращает список трекеров согласно заданному фильтру, условиям сортировки и параметрам постраничного отображения
    /// </summary>
    /// <param name="trackersRequest">Параметры запроса для списка трекеров</param>
    /// <response code="200">Список трекеров успещно возвращен</response>
    /// <response code="400">Невозможно вернуть список трекеров</response>
    [HttpGet("trackers")]
    [ProducesResponseType(typeof(TrackersResponse), StatusCodes.Status200OK)]
    public IActionResult GetTrackers([FromQuery] TrackersRequest trackersRequest)
    {
        ValidationResult validationResult = _trackersRequestValidator
            .Validate(trackersRequest);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var filter = _mapper.Map<TrackersFilter>(trackersRequest);

        var trackersPaging = _trackerRepository.GetTrackers(filter);

        var result = new TrackersResponse
        {
            Trackers = trackersPaging.Results.Select(res => _mapper.Map<TrackerDto>(res)).ToArray(),
            Pagination = new Pagination
            {
                PageIndex = trackersPaging.CurrentPage,
                Total = trackersPaging.TotalPageCount
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// Возвращает трекер  по Id
    /// </summary>
    /// <param name="id">Id трекера</param>
    /// <response code="200">Трекер успещно возвращен</response>
    /// <response code="404">Трекер не найден</response>
    [HttpGet("tracker/{id:int}")]
    [ProducesResponseType(typeof(TrackerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult GetTracker(int id)
    {
        var tracker = _trackerRepository[id];
        if (tracker is not null)
        {
            var trackerDto = _mapper.Map<TrackerDto>(tracker);
            return Ok(trackerDto);
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Трекер с id = {id} не найден"));
        }
    }

    /// <summary>
    /// Добавляет трекер
    /// </summary>
    /// <param name="createTrackerDto">Модель создания трекера</param>
    /// <response code="200">Новый трекер успешно создан, возвращет данные нового трекера</response>
    /// <response code="409">Конфликт при обновлении трекера</response>
    [HttpPost("tracker")]
    [ProducesResponseType(typeof(TrackerDto), StatusCodes.Status200OK)]
    public IActionResult AddTracker(CreateTrackerDto createTrackerDto)
    {
        ValidationResult validationResult = _createValidator.Validate(createTrackerDto);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var newTracker = _mapper.Map<Tracker>(createTrackerDto);
        try
        {
            _trackerRepository.Add(newTracker);
            var trackerDto = _mapper.Map<TrackerDto>(newTracker);
            return Ok(trackerDto);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new ServiceErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// Обновляет трекер
    /// </summary>
    /// <param name="id">Id трекера</param>
    /// <param name="updateTrackerDto">Модель обновления трекера</param>
    /// <response code="200">Трекер успешно обновлен</response>
    /// <response code="404">Трекер не найден</response>
    /// <response code="409">Конфликт при обновлении трекера</response>
    [HttpPut("tracker/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status409Conflict)]
    public IActionResult UpdateTracker(int id, UpdateTrackerDto updateTrackerDto)
    {
        ValidationResult validationResult = _updateValidator.Validate(updateTrackerDto);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        var tracker = _trackerRepository[id];
        if (tracker is not null)
        {
            _mapper.Map(updateTrackerDto, tracker);

            try
            {
                _trackerRepository.Update(tracker);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new ServiceErrorResult(ex.Message));
            }
            return Ok();
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Трекер с id = {id} не найден"));
        }
    }

    /// <summary>
    /// Удаляет трекер
    /// </summary>
    /// <param name="id">Id трекера</param>
    /// <response code="200">Трекер успешно удален</response>
    /// <response code="404">Трекер не найден</response>
    /// <response code="409">Конфликт при удалении трекера</response>
    [HttpDelete("tracker/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult DeleteTracker(int id)
    {
        var tracker = _trackerRepository[id];
        if (tracker is not null)
        {
            try
            {
                _trackerRepository.Remove(tracker);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return Conflict(new ServiceErrorResult(ex.Message));
            }
        }
        else
        {
            return NotFound(new ServiceErrorResult($"Трекер с id = {id} не найден"));
        }
    }
}