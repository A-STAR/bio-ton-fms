﻿using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Dtos;
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
public class TrackerController : ControllerBase
{
    private readonly ILogger<TrackerController> _logger;
    private readonly ITrackerRepository _trackerRepo;
    private readonly IMapper _mapper;

    public TrackerController(
        ITrackerRepository trackerRepo,
        IMapper mapper,
        ILogger<TrackerController> logger)
    {
        _trackerRepo = trackerRepo;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Возвращает список трекеров согласно заданному фильтру, условиям сортировки и параметрам постраничного отображения
    /// </summary>
    /// <param name="trackersRequest">Параметры запроса для списка трекеров</param>
    /// <response code="200">Список трекеров успещно возвращен</response>
    /// <response code="400">Невозможно вернуть список трекеров</response>
    [HttpGet("trackers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrackers([FromQuery] TrackersRequest trackersRequest)
    {
        var filter = _mapper.Map<TrackersFilter>(trackersRequest);

        var trackersPaging = _trackerRepo.GetTrackers(filter);

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
    [HttpGet("tracker/{id}")]
    [ProducesResponseType(typeof(TrackerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTracker(int id)
    {
        var tracker = _trackerRepo[id];

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
    /// <response code="200">Новая трекер успешно создан</response>
    /// <response code="409">Конфликт при обновлении трекера</response>
    [HttpPost("tracker")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddTracker(CreateTrackerDto createTrackerDto)
    {
        var newTracker = _mapper.Map<Tracker>(createTrackerDto);

        try
        {
            _trackerRepo.AddTracker(newTracker);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new ServiceErrorResult(ex.Message));
        }

        return Ok();
    }

    /// <summary>
    /// Обновляет трекер
    /// </summary>
    /// <param name="id">Id трекера</param>
    /// <param name="updateTrackerDto">Модель обновления трекера</param>
    /// <response code="200">Трекер успешно обновлен</response>
    /// <response code="404">Трекер не найден</response>
    /// <response code="409">Конфликт при обновлении трекера</response>
    [HttpPut("tracker/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTracker(int id, UpdateTrackerDto updateTrackerDto)
    {
        var tracker = _trackerRepo[id];

        if (tracker is not null)
        {
            _mapper.Map(updateTrackerDto, tracker);

            try
            {
                _trackerRepo.UpdateTracker(tracker);
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
    [HttpDelete("tracker/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteTracker(int id)
    {
        var tracker = _trackerRepo[id];

        if (tracker is not null)
        {
            try
            {
                _trackerRepo.DeleteTracker(tracker);
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