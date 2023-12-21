using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos;
using BioTonFMS.Telematica.Dtos.MessagesView;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Validation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// API cервиса для работы со статистикой
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica/messagesview")]
[Consumes("application/json")]
[Produces("application/json")]
public class MessagesViewController :  ValidationControllerBase
{
    private readonly ILogger<MessagesViewController> _logger;
    private readonly IMapper _mapper;
    private readonly MessagesViewMessagesRequestValidator _messagesViewMessagesRequestValidator;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ITrackerCommandRepository _commandRepository;
    private readonly ITrackerMessageRepository _messageRepository;

    public MessagesViewController(
        ILogger<MessagesViewController> logger,
        IMapper mapper,
        MessagesViewMessagesRequestValidator messagesViewMessagesRequestValidator,
        IVehicleRepository vehicleRepository,
        ITrackerCommandRepository commandRepository,
        ITrackerMessageRepository messageRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _messagesViewMessagesRequestValidator = messagesViewMessagesRequestValidator;
        _vehicleRepository = vehicleRepository;
        _commandRepository = commandRepository;
        _messageRepository = messageRepository;
    }

    /// <summary>
    /// Просмотр сообщений. Получение статистики.
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ViewMessageStatisticsDto), StatusCodes.Status200OK)]
    public IActionResult GetMessagesViewStatistics([FromQuery] MessagesViewStatisticsRequest request)
    {
        if (!_vehicleRepository.GetExternalIds(request.VehicleId).TryGetValue(request.VehicleId, out var externalId))
        {
            return NotFound("Трекер машины с таким id не существует");
        }

        return Ok(request.ViewMessageType == ViewMessageTypeEnum.DataMessage
            ? _messageRepository.GetStatistics(externalId, request.PeriodStart.ToUniversalTime(),
                request.PeriodEnd.ToUniversalTime())
            : _commandRepository.GetStatistics(externalId, request.PeriodStart.ToUniversalTime(),
                request.PeriodEnd.ToUniversalTime()));
    }

    /// <summary>
    /// Возвращает список машин согласно заданному критерию поиска
    /// </summary>
    /// <param name="findCriterion">Критерий поиска для списка машин</param>
    /// <response code="200">Список машин успешно возвращен</response>
    /// <response code="400">Невозможно вернуть список машин</response>
    [HttpGet("vehicles")]
    [ProducesResponseType(typeof(MessagesViewVehicleDto[]), StatusCodes.Status200OK)]
    public IActionResult FindVehicles([FromQuery] string? findCriterion)
    {
        Vehicle[] vehicles = _vehicleRepository.FindVehicles(findCriterion);

        MessagesViewVehicleDto[] messagesViewVehicleDtos =
            vehicles.Select(v => _mapper.Map<MessagesViewVehicleDto>(v)).ToArray();

        return Ok(messagesViewVehicleDtos);
    }

    /// <summary>
    /// Возвращает точки для трека для выбранной машины и периода
    /// </summary>
    [HttpGet("track")]
    [ProducesResponseType(typeof(LocationsAndTracksResponse), StatusCodes.Status200OK)]
    public IActionResult GetMessagesViewTrack([FromQuery] MessagesViewTrackRequest request)
    {
        if (!_vehicleRepository.GetExternalIds(request.VehicleId).TryGetValue(request.VehicleId, out var externalId))
        {
            return NotFound("Машина с таким id не существует, либо к ней не привязан трекер");
        }

        if (!_messageRepository.GetTracks(request.PeriodStart.ToUniversalTime(),
                    request.PeriodEnd.ToUniversalTime(), externalId)
                .TryGetValue(externalId, out TrackPointInfo[]? points) || points.IsNullOrEmpty())
        {
            return Ok(new LocationsAndTracksResponse { Tracks = ArraySegment<LocationAndTrack>.Empty });
        }

        ViewBounds? viewBounds = TelematicaHelpers.CalculateViewBounds(points);
        string name = _vehicleRepository.GetNamesWhereTrackerNotEmpty(request.VehicleId)[request.VehicleId];
        TrackPointInfo lastPoint = points.Last();

        return Ok(new LocationsAndTracksResponse
        {
            ViewBounds = viewBounds,
            Tracks = new List<LocationAndTrack>
            {
                new()
                {
                    Longitude = lastPoint.Longitude,
                    Latitude = lastPoint.Latitude,
                    VehicleId = request.VehicleId,
                    VehicleName = name,
                    Track = points
                }
            }
        });
    }

    /// <summary>
    /// Возвращает сообщения для выбранной машины и периода
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ViewMessageMessagesDto), StatusCodes.Status200OK)]
    public IActionResult GetMessagesViewMessages([FromQuery] MessagesViewMessagesRequest request)
    {
        ValidationResult validationResult = _messagesViewMessagesRequestValidator
            .Validate(request);
        if (!validationResult.IsValid)
        {
            return ReturnValidationErrors(validationResult);
        }

        if (!_vehicleRepository.GetExternalIds(request.VehicleId)
                .TryGetValue(request.VehicleId, out var externalId))
        {
            return NotFound("Машина с таким id не существует, либо к ней не привязан трекер");
        }

        if (request is
            {
                ViewMessageType: ViewMessageTypeEnum.DataMessage,
                ParameterType: ParameterTypeEnum.TrackerData
            })
        {
            var pagedDataMessages = _messageRepository.GeTrackertDataMessages(externalId, request.PeriodStart.ToUniversalTime(), 
                request.PeriodEnd.ToUniversalTime(), request.PageNum, request.PageSize);
            return Ok(new ViewMessageMessagesDto
            {
                TrackerDataMessages = pagedDataMessages.Results.ToArray(),
                Pagination = new Pagination
                {
                    PageIndex = pagedDataMessages.CurrentPage,
                    Total = pagedDataMessages.TotalPageCount
                }
            });
        }

        if (request is
            {
                ViewMessageType: ViewMessageTypeEnum.DataMessage,
                ParameterType: ParameterTypeEnum.SensorData
            })
        {
        }

        if (request.ViewMessageType == ViewMessageTypeEnum.CommandMessage)
        {
        }

        return Ok();
    }
}