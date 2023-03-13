using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioTonFMS.Telematica.Controllers;

[Authorize]
[ApiController]
[Route("/api/telematica")]
[Consumes("application/json")]
[Produces("application/json")]
public class TrackerDataController : ValidationControllerBase
{
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerRepository _trackerRepository;

    public TrackerDataController(ITrackerMessageRepository messageRepository,
        ITrackerRepository trackerRepository)
    {
        _messageRepository = messageRepository;
        _trackerRepository = trackerRepository;
    }

    /// <summary>
    /// Возвращает список последних полученных стандартных параметров трекера
    /// </summary>
    /// <param name="trackerId">Id трекера</param>
    /// <response code="200">Список стандартных параметров успешно возвращен</response>
    /// <response code="404">Трекера не существует</response>
    [HttpGet("tracker/standard-parameters/{trackerId:int}")]
    [ProducesResponseType(typeof(TrackerStandardParameter[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult GetStandardParameters(int trackerId)
    {
        var tracker = _trackerRepository[trackerId];

        if (tracker is null) return NotFound();

        var parameters = _messageRepository.GetStandardParameters(tracker.ExternalId, tracker.Imei);
        return Ok(parameters.GetArray());
    }

    /// <summary>
    /// Возвращает список последних полученных параметров трекера
    /// </summary>
    /// <param name="trackerId">Id трекера</param>
    /// <response code="200">Список параметров успешно возвращен</response>
    /// <response code="404">Трекера не существует</response>
    [HttpGet("tracker/parameters/{trackerId:int}")]
    [ProducesResponseType(typeof(TrackerParameter[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceErrorResult), StatusCodes.Status404NotFound)]
    public IActionResult GetParameters(int trackerId)
    {
        Tracker? tracker = _trackerRepository[trackerId];

        if (tracker is null) return NotFound();

        IList<TrackerParameter> parameters = _messageRepository.GetParameters(tracker.ExternalId, tracker.Imei);
        return Ok(parameters);
    }
}
