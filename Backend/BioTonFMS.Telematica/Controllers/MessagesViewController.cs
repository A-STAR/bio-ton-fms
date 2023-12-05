using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Domain.MessageStatistics;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos.MessagesView;
using BioTonFMS.Telematica.Dtos.Monitoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// API cервиса для работы со статистикой
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica/messagesview")]
[Consumes("application/json")]
[Produces("application/json")]
public class MessagesViewController : ControllerBase
{
    private readonly ILogger<MessagesViewController> _logger;
    private readonly IMapper _mapper;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ITrackerCommandRepository _commandRepository;
    private readonly ITrackerMessageRepository _messageRepository;
    
    public MessagesViewController(
        ILogger<MessagesViewController> logger,
        IMapper mapper,
        IVehicleRepository vehicleRepository,
        ITrackerCommandRepository commandRepository,
        ITrackerMessageRepository messageRepository)
    {
        _logger = logger;
        _mapper = mapper;
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
            ? _messageRepository.GetStatistics(externalId, request.PeriodStart.ToUniversalTime(), request.PeriodEnd.ToUniversalTime())
            : _commandRepository.GetStatistics(externalId, request.PeriodStart.ToUniversalTime(), request.PeriodEnd.ToUniversalTime()));
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

        MessagesViewVehicleDto[] messagesViewVehicleDtos =
            vehicles.Select(v => _mapper.Map<MessagesViewVehicleDto>(v)).ToArray();

        return Ok(messagesViewVehicleDtos);
    }
    
    /// <summary>
    /// Возвращает точки для трека для выбранной машины и периода
    /// </summary>
    [HttpGet("track")]
    [ProducesResponseType(typeof(MessagesViewTrackResponse), StatusCodes.Status200OK)]
    public IActionResult GetMessagesViewTrack([FromQuery] MessagesViewTrackRequest request)
    {
        if (!_vehicleRepository.GetExternalIds(request.VehicleId).TryGetValue(request.VehicleId, out var externalId))
        {
            return NotFound("Машина с таким id не существует, либо к ней не привязан трекер");
        }
        
        if (!_messageRepository.GetTracks(request.PeriodStart.ToUniversalTime(),
                    request.PeriodEnd.ToUniversalTime(), externalId)
            .TryGetValue(externalId, out var points))
        {
            return Ok(new MessagesViewTrackResponse());
        }

        ViewBounds? viewBounds = TelematicaHelpers.CalculateViewBounds(points);

        return Ok(new MessagesViewTrackResponse
        {
            ViewBounds = viewBounds,
            Track = points
        });
    }
}