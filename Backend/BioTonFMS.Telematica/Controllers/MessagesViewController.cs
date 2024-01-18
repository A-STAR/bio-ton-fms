using AutoMapper;
using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Services;
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
public class MessagesViewController : ValidationControllerBase
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
    /// <param name="request">Параметры запроса для получения трека</param>
    /// <response code="200">Трек успешно возвращён</response>
    /// <response code="400">Невалидные параметры запроса</response>
    [HttpGet("track")]
    [ProducesResponseType(typeof(LocationsAndTracksResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
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
    /// <param name="request">Параметры запроса для списка сообщений</param>
    /// <response code="200">Список сообщенний успешно возвращён</response>
    /// <response code="400">Невалидные параметры запроса</response>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ViewMessageMessagesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status400BadRequest)]
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
            PagedResult<TrackerDataMessageDto> pagedDataMessages = _messageRepository.GetParameterDataTrackerMessages(externalId, request.PeriodStart.ToUniversalTime(),
                request.PeriodEnd.ToUniversalTime(), request.PageNum, request.PageSize);
            return Ok(new ViewMessageMessagesDto
            {
                TrackerDataMessages = pagedDataMessages.Results.ToArray(),
                Pagination = new Pagination
                {
                    PageIndex = pagedDataMessages.CurrentPage,
                    Total = pagedDataMessages.TotalPageCount,
                    Records = pagedDataMessages.TotalRowCount
                }
            });
        }

        if (request is
            {
                ViewMessageType: ViewMessageTypeEnum.DataMessage,
                ParameterType: ParameterTypeEnum.SensorData
            })
        {
            PagedResult<SensorDataMessageDto> pagedSensorMessages = _messageRepository.GetSensorDataTrackerMessages(externalId, request.PeriodStart.ToUniversalTime(),
                request.PeriodEnd.ToUniversalTime(), request.PageNum, request.PageSize);
            return Ok(new ViewMessageMessagesDto
            {
                SensorDataMessages = pagedSensorMessages.Results.ToArray(),
                Pagination = new Pagination
                {
                    PageIndex = pagedSensorMessages.CurrentPage,
                    Total = pagedSensorMessages.TotalPageCount,
                    Records = pagedSensorMessages.TotalRowCount
                }
            });
        }

        if (request.ViewMessageType == ViewMessageTypeEnum.CommandMessage)
        {
            PagedResult<CommandMessageDto> pagedCommandMessages = _commandRepository.GetCommandMessages(externalId, request.PeriodStart.ToUniversalTime(),
                request.PeriodEnd.ToUniversalTime(), request.PageNum, request.PageSize);
            return Ok(new ViewMessageMessagesDto
            {
                CommandMessages = pagedCommandMessages.Results.ToArray(),
                Pagination = new Pagination
                {
                    PageIndex = pagedCommandMessages.CurrentPage,
                    Total = pagedCommandMessages.TotalPageCount,
                    Records = pagedCommandMessages.TotalRowCount
                }
            });
        }

        return Ok();
    }

    /// <summary>
    /// Удаляет список сообщений трекеров по идентификаторам
    /// </summary>
    /// <param name="messageIds">Список идентификаторов сообщений предназначенных для удаления</param>
    /// <response code="200">Сообщения успешно удалены</response>
    /// <response code="404">В списке есть идентификаторы несуществующих сообщений</response>
    [HttpDelete("delete-messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult DeleteMessages([FromBody] long[] messageIds)
    {
        if (messageIds.Length == 0)
        {
            return NotFound("Пустой список идентификаторов для удаления");
        }
        try
        {
            _messageRepository.DeleteMessages(messageIds);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new ServiceErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении сообщений");
            throw;
        }
    }

    /// <summary>
    /// Удаляет список команд для трекеров по идентификаторам
    /// </summary>
    /// <param name="messageIds">Список идентификаторов команд для трекеров предназначенных для удаления</param>
    /// <response code="200">Команды успешно удалены</response>
    /// <response code="404">В списке есть идентификаторы несуществующих сообщений</response>
    [HttpDelete("delete-command-messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult DeleteCommandMessages([FromBody] int[] messageIds)
    {
        if (messageIds.Length == 0)
        {
            return NotFound("Пустой список идентификаторов для удаления");
        }
        try
        {
            _commandRepository.DeleteCommands(messageIds);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new ServiceErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении сообщений");
            throw;
        }
    }
}