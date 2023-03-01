using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
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

    public TrackerDataController(ITrackerMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    /// <summary>
    /// Возвращает список последних полученных стандартных параметров трекера
    /// </summary>
    /// <param name="trackerId">Id трекера</param>
    /// <response code="200">Список стандартных успешно возвращен</response>
    [HttpGet("tracker/standardparameters/{trackerId:int}")]
    public ICollection<TrackerStandardParameter> GetStandardParameters(int trackerId)
    {
        return _messageRepository.GetParameters(trackerId).GetArray();
    }
}