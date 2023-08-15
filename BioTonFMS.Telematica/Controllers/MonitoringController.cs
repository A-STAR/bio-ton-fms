using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos.Monitoring;
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
[Route("/api/telematica/monitoring")]
[Consumes("application/json")]
[Produces("application/json")]
public class MonitoringController : ValidationControllerBase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly ITrackerMessageRepository _messageRepository;

    public MonitoringController(
        IMapper mapper,
        ILogger<MonitoringController> logger,
        IVehicleRepository vehicleRepository, ITrackerMessageRepository messageRepository)
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

        MonitoringVehicleDto[] monitoringDtos = vehicles.Select(v => _mapper.Map<MonitoringVehicleDto>(v)).ToArray();

        int[] trackerExternalIds = monitoringDtos.Where(x => x.Tracker?.ExternalId != null)
            .Select(x => x.Tracker!.ExternalId!.Value).ToArray();

        var states = _messageRepository.GetVehicleStates(trackerExternalIds, 60);

        foreach (MonitoringVehicleDto monitoringDto in monitoringDtos)
        {
            if (monitoringDto.Tracker?.ExternalId == null) continue;

            if (!states.TryGetValue(monitoringDto.Tracker.ExternalId.Value, out var status))
            {
                status = new VehicleStatus
                {
                    TrackerExternalId = monitoringDto.Tracker.ExternalId.Value,
                    MovementStatus = MovementStatusEnum.NoData,
                    ConnectionStatus = ConnectionStatusEnum.NotConnected
                };
            }

            monitoringDto.MovementStatus = status.MovementStatus;
            monitoringDto.ConnectionStatus = status.ConnectionStatus;
            monitoringDto.LastMessageTime = status.LastMessageTime;
            monitoringDto.NumberOfSatellites = status.NumberOfSatellites;
        }

        return Ok(monitoringDtos);
    }

    /// <summary>
    /// Возвращает текущие координаты и данные для построения быстрого трека для заданных машин
    /// </summary>
    /// <response code="200">Данные успешно возвращены</response>
    /// <response code="400">Невозможно вернуть данные</response>
    [HttpPost("locations-and-tracks")]
    [ProducesResponseType(typeof(LocationsAndTracksResponse), StatusCodes.Status200OK)]
    public IActionResult LocationsAndTracks(LocationAndTrackRequest[] requests)
    {
        var externalIds = _vehicleRepository.GetExternalIds(
            requests.Select(x => x.VehicleId).ToArray());

        var locations = _messageRepository.GetLocations(externalIds.Values.ToArray());

        var needTrack = requests.Where(x => x.NeedReturnTrack)
            .Select(x => x.VehicleId);

        var tracks = _messageRepository.GetTracks(
            externalIds.Where(x => needTrack.Contains(x.Key))
                .Select(x => x.Value)
                .ToArray()
        );

        var locationsAndTracks = new List<LocationAndTrack>();
        foreach (var r in requests)
        {
            if (!externalIds.TryGetValue(r.VehicleId, out var externalId)) continue;
            
            if (!locations.TryGetValue(externalId, out var location)) continue;

            var locationAndTrack = new LocationAndTrack
            {
                Longitude = location.Item1,
                Latitude = location.Item2,
                VehicleId = r.VehicleId
            };

            if (!r.NeedReturnTrack || !tracks.TryGetValue(externalId, out var track))
            {
                locationAndTrack.Track = Array.Empty<TrackPointInfo>();
            }
            else
            {
                locationAndTrack.Track = track;
            }
            
            locationsAndTracks.Add(locationAndTrack);
        }

        var lons = locationsAndTracks.SelectMany(x => x.Track).Select(x => x.Longitude).ToList();
        lons.AddRange(locationsAndTracks.Select(x => x.Longitude));
        
        var lats = locationsAndTracks.SelectMany(x => x.Track).Select(x => x.Latitude).ToList();
        lats.AddRange(locationsAndTracks.Select(x => x.Latitude));

        var difLat = (lats.Max() - lats.Min()) / 20;
        var difLon = (lons.Max() - lons.Min()) / 20;

        return Ok(new LocationsAndTracksResponse
        {
            ViewBounds = new ViewBounds
            {
                UpperLeftLatitude = lats.Max() + difLat,
                UpperLeftLongitude = lons.Min() - difLon,
                BottomRightLatitude = lats.Min() - difLat,
                BottomRightLongitude = lats.Max() + difLon
            },
            Tracks = locationsAndTracks
        });
    }
}