using BioTonFMS.Domain.MessageStatistics;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.Telematica.Dtos.MessagesView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioTonFMS.Telematica.Controllers;

/// <summary>
/// API cервиса для работы со статистикой
/// </summary>
[Authorize]
[ApiController]
[Route("/api/telematica")]
[Consumes("application/json")]
[Produces("application/json")]
public class MessagesViewController : ControllerBase
{
    public IVehicleRepository VehicleRepository { get; set; }
    public ITrackerCommandRepository CommandRepository { get; set; }
    public ITrackerMessageRepository MessageRepository { get; set; }
    
    public MessagesViewController(IVehicleRepository vehicleRepository,
        ITrackerCommandRepository commandRepository,
        ITrackerMessageRepository messageRepository)
    {
        VehicleRepository = vehicleRepository;
        CommandRepository = commandRepository;
        MessageRepository = messageRepository;
    }
    
    /// <summary>
    /// Просмотр сообщений. Получение статистики.
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ViewMessageStatisticsDto), StatusCodes.Status200OK)]
    public IActionResult GetMessagesViewStatistics([FromQuery] MessagesViewStatisticsRequest request)
    {
        var externalId = VehicleRepository.GetExternalIds(request.VehicleId).Values.FirstOrDefault();
        
        if (externalId == null)
        {
            return NotFound("Трекер машины с таким id не существует");
        }

        return Ok(request.ViewMessageType == ViewMessageTypeEnum.DataMessage
            ? MessageRepository.GetStatistics(externalId, request.PeriodStart, request.PeriodEnd)
            : CommandRepository.GetStatistics(externalId, request.PeriodStart, request.PeriodEnd));
    }
}