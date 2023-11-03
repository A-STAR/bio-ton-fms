using AutoMapper;
using BioTonFMS.Common.Settings;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos.MessagesView;
using BioTonFMS.Telematica.Dtos.Monitoring;
using BioTonFMS.Telematica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// API cервиса просмотра сообщений
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica/messagesview")]
[Consumes("application/json")]
[Produces("application/json")]
public class MessagesViewController : ValidationControllerBase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly ITrackerMessageRepository _messageRepository;

    public MessagesViewController(
        IMapper mapper,
        ILogger<MessagesViewController> logger,
        IVehicleRepository vehicleRepository,
        ITrackerMessageRepository messageRepository)
    {
        _vehicleRepository = vehicleRepository;
        _messageRepository = messageRepository;
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

        MessagesViewVehicleDto[] messagesViewVehicleDtos = vehicles.Select(v => _mapper.Map<MessagesViewVehicleDto>(v)).ToArray();

        return Ok(messagesViewVehicleDtos);
    }
    
    /// <summary>
    /// Возвращает точки для трека для выбранной машины и периода
    /// </summary>
    [HttpGet("track")]
    [ProducesResponseType(typeof(MessagesViewTrackResponse), StatusCodes.Status200OK)]
    public IActionResult GetMessagesViewTrack([FromQuery] int vehicleId,
        [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd)
    {
        if (!_vehicleRepository.GetExternalIds(vehicleId).TryGetValue(vehicleId, out var externalId))
        {
            return NotFound("Машина с таким id не существует, либо к ней не привязан трекер");
        }
        
        if (!_messageRepository.GetTracks(periodStart, periodEnd, externalId)
            .TryGetValue(externalId, out var points))
        {
            return Ok(new MessagesViewTrackResponse());
        }

        ViewBounds? viewBounds = CalculateViewBounds(points);

        return Ok(new MessagesViewTrackResponse
        {
            ViewBounds = viewBounds,
            Track = points
        });
    }

    #region Private

    private static ViewBounds? CalculateViewBounds(ICollection<TrackPointInfo> points)
    {
        if (points.Count == 0)
        {
            return null;
        }
        
        List<double> lons = points.Select(x => x.Longitude).ToList();
        List<double> lats = points.Select(x => x.Latitude).ToList();

        var difLat = (lats.Max() - lats.Min()) / 20;
        var difLon = (lons.Max() - lons.Min()) / 20;

        if (difLat < MonitoringController.DefaultDifLat)
        {
            difLat = MonitoringController.DefaultDifLat;
        }
        if (difLon < MonitoringController.DefaultDifLon)
        {
            difLon = MonitoringController.DefaultDifLon;
        }

        var viewBounds = new ViewBounds
        {
            UpperLeftLatitude = lats.Max() + difLat,
            UpperLeftLongitude = lons.Min() - difLon,
            BottomRightLatitude = lats.Min() - difLat,
            BottomRightLongitude = lons.Max() + difLon
        };
        return viewBounds;
    }

    #endregion
}