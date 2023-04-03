using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Services;
using BioTonFMS.Telematica.Dtos.TrackerCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers
{
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

        public TrackerCommandController(
            ILogger<TrackerCommandController> logger,
            ITrackerRepository trackerRepository)
        {
            _logger = logger;
            _trackerRepository = trackerRepository;
        }

        /// <summary>
        /// Отправляет команду на трекер и возвращает ответ от трекера
        /// </summary>
        /// <param name="trackerId">Идентификатор трекера которому отправляется команда</param>
        /// <param name="сommandDto">Команда для трекера</param>
        /// <response code="200">Команда успешно отправлена на трекер, возвращет ответ от трекера</response>
        /// <response code="404">Трекер не найден</response>
        /// <response code="409">Конфликт при отправке команды</response>
        [HttpPost("tracker-command/{trackerId:int}")]
        [ProducesResponseType(typeof(TrackerCommandResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status409Conflict)]
        public IActionResult SendCommand(int trackerId, TrackerCommandDto сommandDto)
        {
            if (сommandDto.Transport != TrackerCommandTransportEnum.TCP)
            {
                return Conflict(new ServiceErrorResult("Такой способ доставки комманд не поддерживается"));
            }
            var response = new TrackerCommandResponseDto();
            var tracker = _trackerRepository[trackerId];
            if (tracker is not null)
            {
                
                switch (сommandDto.CommandText.ToUpper())
                {
                    case "IMEI":
                        response.CommandResponse = "IMEI 123456789012345,12345678901234567890";
                        break;
                    case "INFO":
                        response.CommandResponse = "5.0,M8Q,231.33,8247";
                        break;
                    case "RESET":
                        response.CommandResponse = "Reset of device. Please wait 15 seconds…";
                        break;
                    default:
                        response.CommandResponse = "Reset of device. Please wait 15 seconds…";
                        break;
                }
            }
            else
            {
                return NotFound(new ServiceErrorResult($"Трекер с id = {trackerId} не найден"));
            }
            return Ok(response);
        }
    }
}
