using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using Microsoft.AspNetCore.Authorization;
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
    /// <response code="200">Список стандартных успешно возвращен</response>
    /// <response code="404">Трекера не существует</response>
    [HttpGet("tracker/standardparameters/{trackerId:int}")]
    public IActionResult GetStandardParameters(int trackerId)
    {
        var tracker = _trackerRepository[trackerId];

        if (tracker is null) return NotFound();

        var parameters = _messageRepository.GetParameters(tracker.ExternalId, tracker.Imei);
        return Ok(parameters.GetArray());
    }
}