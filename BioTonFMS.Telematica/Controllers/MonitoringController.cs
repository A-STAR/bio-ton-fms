using AutoMapper;
using BioTonFMS.Common.Settings;
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
    public IActionResult LocationsAndTracks(LocationAndTrackRequest[] requests)
    {
        IDictionary<int, int> externalIds = _vehicleRepository.GetExternalIds(
            requests.Select(x => x.VehicleId).ToArray());

        IDictionary<int, (double Lat, double Long)> locations = _messageRepository.GetLocations(externalIds.Values.ToArray());

        IEnumerable<int> needTrack = requests.Where(x => x.NeedReturnTrack)
            .Select(x => x.VehicleId);

        IDictionary<int, TrackPointInfo[]> tracks = _messageRepository.GetTracks(
            externalIds.Where(x => needTrack.Contains(x.Key))
                .Select(x => x.Value)
                .ToArray()
        );

        List<LocationAndTrack> locationsAndTracks = GetLocationsAndTracks(requests, externalIds, locations, tracks);

        ViewBounds viewBounds = CalculateViewBounds(locationsAndTracks);

        return Ok(new LocationsAndTracksResponse
        {
            ViewBounds = viewBounds,
            Tracks = locationsAndTracks
        });
    }

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
            BottomRightLongitude = lats.Max() + difLon
        };
        return viewBounds;
    }

    [HttpGet("vehicle/{id:int}")]
    [ProducesResponseType(typeof(MonitoringVehicleInfoDto), StatusCodes.Status200OK)]
    public IActionResult GetVehicleInformation(int id)
    {
        return Ok(new MonitoringVehicleInfoDto
        {
            GeneralInfo = new MonitoringGeneralInfoDto
            {
                Latitude = 36.884816,
                Longitude = 30.701173,
                LastMessageTime = DateTime.UtcNow.AddMinutes(-25),
                Mileage = 73000,
                EngineHours = 246,
                Speed = 73,
                SatellitesNumber = 21
            },
            TrackerInfo = new MonitoringTrackerInfoDto
            {
                ExternalId = -1,
                Imei = "6C6F6C6B656B636",
                SimNumber = "88005553535",
                TrackerType = "GalileoSkyV50",
                Parameters = new[]
                {
                    new TrackerParameter
                    {
                        ParamName = "param decimal",
                        LastValueDecimal = 123.12
                    },
                    new TrackerParameter
                    {
                        ParamName = "param str",
                        LastValueString = "somthing"
                    },
                    new TrackerParameter
                    {
                        ParamName = "param date",
                        LastValueDateTime = DateTime.UnixEpoch.AddDays(3304)
                    },
                },
                Sensors = new[]
                {
                    new TrackerSensorDto
                    {
                        Name = "sensor 1",
                        Value = "val",
                        Unit = "unit"
                    },
                    new TrackerSensorDto
                    {
                        Name = "sensor 2",
                        Value = "123",
                        Unit = "miles"
                    },
                }
            }
        });
    }
}