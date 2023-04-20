using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.TrackerCommands.Exceptions;
using BioTonFMS.TrackerCommands.Senders;
using BioTonFMS.Telematica.Dtos.TrackerCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// API cервиса для отправки команд трекерам
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica")]
[Consumes("application/json")]
[Produces("application/json")]
public class TrackerCommandController : ValidationControllerBase
{
    private readonly ILogger<TrackerCommandController> _logger;
    private readonly ITrackerRepository _trackerRepository;
    private readonly ITrackerCommandRepository _trackerCommandRepository;
    private readonly Func<TrackerTypeEnum, TrackerCommandTransportEnum, ITrackerCommandSender?> _senderResolver;

    public TrackerCommandController(
        ILogger<TrackerCommandController> logger,
        ITrackerRepository trackerRepository,
        Func<TrackerTypeEnum, TrackerCommandTransportEnum, ITrackerCommandSender?> senderResolver,
        ITrackerCommandRepository trackerCommandRepository)
    {
        _logger = logger;
        _trackerRepository = trackerRepository;
        _senderResolver = senderResolver;
        _trackerCommandRepository = trackerCommandRepository;
    }

    /// <summary>
    /// Отправляет команду на трекер и возвращает ответ от трекера
    /// </summary>
    /// <param name="trackerId">Идентификатор трекера которому отправляется команда</param>
    /// <param name="commandDto">Команда для трекера</param>
    /// <response code="200">Команда успешно отправлена на трекер, возвращает ответ от трекера</response>
    /// <response code="404">Трекер не найден</response>
    /// <response code="409">Конфликт при отправке команды</response>
    [HttpPost("tracker-command/{trackerId:int}")]
    [ProducesResponseType(typeof(TrackerCommandResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status409Conflict)]
    public IActionResult SendCommand(int trackerId, TrackerCommandDto commandDto)
    {
        var sender = _senderResolver(TrackerTypeEnum.GalileoSkyV50, commandDto.Transport);
        if (sender is null)
        {
            return Conflict(new ServiceErrorResult("Такой способ доставки команд не поддерживается"));
        }

        var tracker = _trackerRepository[trackerId];
        if (tracker is null)
        {
            return NotFound(new ServiceErrorResult($"Трекер с id = {trackerId} не найден"));
        }

        try
        {
            var response = sender.Send(tracker, commandDto.CommandText);

            var command = new TrackerCommand
            {
                ResponseText = response.ResponseText,
                CommandText = commandDto.CommandText,
                TrackerId = trackerId,
                BinaryResponse = response.ResponseBinaryInfo,
                SentDateTime = SystemTime.UtcNow
            };
            
            _trackerCommandRepository.Add(command);

            return Ok(new TrackerCommandResponseDto
            {
                CommandResponse = response.ResponseText
            });
        }
        catch (NotValidTrackerAddressException e)
        {
            return Conflict(new ServiceErrorResult(e.Message));
        }
        catch (TimeoutException e)
        {
            return Conflict(new ServiceErrorResult(e.Message));
        }
        catch (ParseCommandResponseException e)
        {
            return Conflict(new ServiceErrorResult(e.Message));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при отправке команды {Command} {Transport} трекеру id = {Tracker}",
                commandDto.CommandText, commandDto.Transport.GetDescription(), trackerId);
            return Conflict(new ServiceErrorResult("Ошибка при отправке команды"));
        }
    }
}