using BioTonFMS.Common.Settings;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Extensions;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.TrackerProtocolSpecific.Exceptions;
using BioTonFMS.TrackerProtocolSpecific.Senders;
using BioTonFMS.Telematica.Dtos.TrackerCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
    private readonly TrackerOptions _options;
    private readonly Func<TrackerTypeEnum, TrackerCommandTransportEnum, ITrackerCommandSender?> _senderResolver;

    const int delayMs = 5000;

    public TrackerCommandController(
        ILogger<TrackerCommandController> logger,
        ITrackerRepository trackerRepository,
        Func<TrackerTypeEnum, TrackerCommandTransportEnum, ITrackerCommandSender?> senderResolver,
        ITrackerCommandRepository trackerCommandRepository, IOptions<TrackerOptions> options)
    {
        _logger = logger;
        _trackerRepository = trackerRepository;
        _senderResolver = senderResolver;
        _trackerCommandRepository = trackerCommandRepository;
        _options = options.Value;
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
    public async Task<IActionResult> SendCommand(int trackerId, TrackerCommandDto commandDto)
    {
        ITrackerCommandSender? sender = _senderResolver(TrackerTypeEnum.GalileoSkyV50, commandDto.Transport);
        if (sender is null)
        {
            return Conflict(new ServiceErrorResult("Такой способ доставки команд не поддерживается"));
        }

        Tracker? tracker = _trackerRepository[trackerId];
        if (tracker is null)
        {
            return NotFound(new ServiceErrorResult($"Трекер с id = {trackerId} не найден"));
        }

        try
        {
            TrackerCommand command = new TrackerCommand
            {
                CommandText = commandDto.CommandText,
                Tracker = tracker,
                SentDateTime = SystemTime.UtcNow
            };
            _trackerCommandRepository.Add(command);
            sender.Send(command);
            _logger.LogDebug("TrackerCommandController.SendCommand отправлена команда с Id = {CommandId}", command.Id);

            TrackerCommand? updatedCommand = null;
            var readTask = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(delayMs);
                    updatedCommand = _trackerCommandRepository.GetWithoutCaching(command!.Id);
                    _logger.LogTrace("TrackerCommandController.SendCommand проверка команды с с Id = {CommandId} ResponseDateTime = {ResponseDateTime}, updatedCommand = {updatedCommand}",
                        command.Id, updatedCommand?.ResponseDateTime, updatedCommand);
                    if (updatedCommand?.ResponseDateTime != null)
                    {
                        _logger.LogDebug("TrackerCommandController.SendCommand проверка команды с с Id = {CommandId} ответ получен", command.Id);
                        break;
                    }
                }
            });
            
            var timeoutTask = Task.Delay(_options.CommandTimeoutSec * 1000);
            var success = await Task.WhenAny(readTask, timeoutTask) == readTask;
            
            if (success && updatedCommand != null)
            {
                _logger.LogDebug("TrackerCommandController.SendCommand проверка команды с с Id = {CommandId} ответ = {ResponseText}", command.Id, updatedCommand.ResponseText);
                return Ok(new TrackerCommandResponseDto
                {
                    CommandResponse = updatedCommand.ResponseText
                });
            }

            return Conflict(new ServiceErrorResult("Ответ не получен в течении заданного времени"));
        }
        catch (NotValidTrackerAddressException e)
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