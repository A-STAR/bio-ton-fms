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

    [HttpGet("tracker/standardparameters/{id:int}")]
    public ICollection<TrackerStandardParameter> GetStandardParameters(int id)
    {
        return _messageRepository.GetParameters(id).GetArray();
    }
}