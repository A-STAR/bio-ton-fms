using AutoMapper;
using BioTonFMS.Common.Settings;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos.Monitoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
    private readonly TrackerOptions _trackerOptions;

    public MonitoringController(
        IMapper mapper,
        ILogger<MonitoringController> logger,
        IVehicleRepository vehicleRepository,
        ITrackerMessageRepository messageRepository,
        IOptions<TrackerOptions> trackerOptions)
    {
        _vehicleRepository = vehicleRepository;
        _messageRepository = messageRepository;
        _trackerOptions = trackerOptions.Value;
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

        var states = _messageRepository.GetVehicleStates(trackerExternalIds,
            _trackerOptions.TrackerAddressValidMinutes);

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
    public IActionResult LocationsAndTracks([FromQuery] DateTime trackStartTime,
        [FromBody] LocationAndTrackRequest[] requests)
    {
        IDictionary<int, int> externalIds = _vehicleRepository.GetExternalIds(
            requests.Select(x => x.VehicleId).ToArray());

        IDictionary<int, (double Lat, double Long)> locations = _messageRepository.GetLocations(externalIds.Values.ToArray());

        IEnumerable<int> vehiclesNeedTrack = requests.Where(x => x.NeedReturnTrack).Select(x => x.VehicleId);

        IDictionary<int, TrackPointInfo[]> tracks = _messageRepository.GetTracks(trackStartTime, 
            externalIds.Where(x => vehiclesNeedTrack.Contains(x.Key))
                .Select(x => x.Value)
                .ToArray()
        );

        List<LocationAndTrack> locationsAndTracks = GetLocationsAndTracks(requests, externalIds, locations, tracks);

        ViewBounds? viewBounds = CalculateViewBounds(locationsAndTracks);

        return Ok(new LocationsAndTracksResponse
        {
            ViewBounds = viewBounds,
            Tracks = locationsAndTracks
        });
    }

    /// <summary>
    /// Возвращает общую информацию и данные трекеров
    /// </summary>
    [HttpGet("vehicle/{id:int}")]
    [ProducesResponseType(typeof(MonitoringVehicleInfoDto), StatusCodes.Status200OK)]
    public IActionResult GetVehicleInformation([FromRoute] int id)
    {
        var tracker = _vehicleRepository.GetTracker(id);

        if (tracker == null)
        {
            return NotFound();
        }
        
        var trackerInfo = _mapper.Map<MonitoringTrackerInfoDto>(tracker);
        trackerInfo.Parameters = _messageRepository.GetParameters(tracker.ExternalId);

        var lastMessage = _messageRepository.GetLastMessageFor(tracker.ExternalId);

        if (lastMessage != null) return Ok(GetVehicleInfo(lastMessage, tracker.Sensors, trackerInfo));
        
        trackerInfo.Sensors = tracker.Sensors
            .Select(x => new TrackerSensorDto
            {
                Name = x.Name, Unit = x.Unit.Name
            })
            .ToList();
            
        return Ok(new MonitoringVehicleInfoDto
        {
            TrackerInfo = trackerInfo,
            GeneralInfo = new MonitoringGeneralInfoDto()
        });
    }

    #region Private

    private static List<LocationAndTrack> GetLocationsAndTracks(LocationAndTrackRequest[] requests, 
        IDictionary<int, int> externalIds, IDictionary<int, (double Lat, double Long)> locations, 
        IDictionary<int, TrackPointInfo[]> tracks)
    {
        var locationsAndTracks = new List<LocationAndTrack>();
        foreach (var request in requests)
        {
            if (!externalIds.TryGetValue(request.VehicleId, out var externalId)) continue;

            if (!locations.TryGetValue(externalId, out var location)) continue;

            var locationAndTrack = new LocationAndTrack
            {
                Longitude = location.Long,
                Latitude = location.Lat,
                VehicleId = request.VehicleId
            };

            if (!request.NeedReturnTrack || !tracks.TryGetValue(externalId, out var track))
            {
                locationAndTrack.Track = Array.Empty<TrackPointInfo>();
            }
            else
            {
                locationAndTrack.Track = track;
            }

            locationsAndTracks.Add(locationAndTrack);
        }

        return locationsAndTracks;
    }

    private static ViewBounds? CalculateViewBounds(List<LocationAndTrack> locationsAndTracks)
    {
        if (locationsAndTracks.Count == 0)
        {
            return null;
        }
        var lons = locationsAndTracks.SelectMany(x => x.Track).Select(x => x.Longitude).ToList();
        lons.AddRange(locationsAndTracks.Select(x => x.Longitude));

        var lats = locationsAndTracks.SelectMany(x => x.Track).Select(x => x.Latitude).ToList();
        lats.AddRange(locationsAndTracks.Select(x => x.Latitude));

        var difLat = (lats.Max() - lats.Min()) / 20;
        var difLon = (lons.Max() - lons.Min()) / 20;

        var viewBounds = new ViewBounds
        {
            UpperLeftLatitude = lats.Max() + difLat,
            UpperLeftLongitude = lons.Min() - difLon,
            BottomRightLatitude = lats.Min() - difLat,
            BottomRightLongitude = lons.Max() + difLon
        };
        return viewBounds;
    }

    private MonitoringVehicleInfoDto GetVehicleInfo(TrackerMessage lastMessage,
        IEnumerable<Sensor> sensors, MonitoringTrackerInfoDto trackerInfo)
    {
        var generalInfo = _mapper.Map<MonitoringGeneralInfoDto>(lastMessage);

        var trackerSensors = sensors.ToDictionary(x => x.Id,
            x => new TrackerSensorDto
            {
                Name = x.Name,
                Unit = x.Unit.Name
            });

        foreach (MessageTag tag in lastMessage.Tags)
        {
            if (tag.SensorId != null && trackerSensors.TryGetValue(tag.SensorId.Value, out var sensorDto))
            {
                sensorDto.Value = tag.ValueString;
            }

            // вычисление пробега и моточасов
            switch (tag.TrackerTagId)
            {
                case TagsSeed.CanB0:
                    generalInfo.Mileage = ((MessageTagInteger)tag).Value * 5;
                    break;
                case TagsSeed.Can32BitR0Id:
                    generalInfo.EngineHours = ((MessageTagInteger)tag).Value / 100;
                    break;
            }
        }

        trackerInfo.Sensors = trackerSensors.Values.ToList();

        return new MonitoringVehicleInfoDto
        {
            GeneralInfo = generalInfo,
            TrackerInfo =  trackerInfo
        };
    }

    #endregion
}