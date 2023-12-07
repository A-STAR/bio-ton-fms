using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos;
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
    [ProducesResponseType(typeof(MessagesViewTrackResponse), StatusCodes.Status200OK)]
    public IActionResult GetMessagesViewTrack([FromQuery] int vehicleId,
        [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd)
    {
        if (!_vehicleRepository.GetExternalIds(vehicleId).TryGetValue(vehicleId, out var externalId))
        {
            return NotFound("Машина с таким id не существует, либо к ней не привязан трекер");
        }
        
        if (!_messageRepository.GetTracks(periodStart.ToUniversalTime(), periodEnd.ToUniversalTime(), externalId)
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

    /// <summary>
    /// Возвращает точки для трека для выбранной машины и периода
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ViewMessageMessagesDto), StatusCodes.Status200OK)]
    public IActionResult GetMessagesViewMessages([FromQuery] MessagesViewMessagesRequest request)
    {
        return Ok(new ViewMessageMessagesDto
        {
            Pagination = new Pagination{ Total = 10, PageIndex = 1},
            CommandMessages = new[]
            {
                new CommandMessageDto
                {
                    Channel = "channel",
                    Num = 1,
                    CommandText = "text 1",
                    ExecutionTime = 123,
                    CommandDateTime = DateTime.UtcNow,
                    CommandResponseText = "response 1"
                },
                new CommandMessageDto
                {
                    Channel = "channel",
                    Num = 2,
                    CommandText = "text 2",
                    ExecutionTime = 542,
                    CommandDateTime = DateTime.UtcNow,
                    CommandResponseText = "response 2"
                }
            },
            SensorDataMessages = new[]
            {
                new SensorDataMessageDto
                {
                    Id = 1,
                    Longitude = 13.13,
                    Latitude = 23.1,
                    ServerDateTime = DateTime.Now,
                    TrackerDateTime = DateTime.Now,
                    Num = 1,
                    Altitude = 23.1,
                    SatNumber = 123,
                    Speed = 44,
                    Sensors = new []
                    {
                        new TrackerSensorDto
                        {
                            Value = "val",
                            Name = "name",
                            Unit = "unit"
                        }
                    }
                }
            },
            TrackerDataMessages = new[]
            {
                new TrackerDataMessageDto
                {
                    Id = 1,
                    Longitude = 12.13,
                    Latitude = 42.152,
                    ServerDateTime = DateTime.Now,
                    TrackerDateTime = DateTime.Now,
                    Num = 178,
                    Altitude = 23.1,
                    SatNumber = 2,
                    Speed = 66,
                    Parameters = new []
                    {
                        new TrackerParameter
                        {
                            ParamName = "param0",
                            LastValueDateTime = DateTime.MaxValue
                        },
                        new TrackerParameter
                        {
                            ParamName = "param1",
                            LastValueDecimal = 34.66
                        },
                        new TrackerParameter
                        {
                            ParamName = "param2",
                            LastValueString = "val"
                        },
                    }
                }
            }
        });
    }
}