using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers.TestData;

/// <summary>
/// Сервис тестовых данных. Генерирует и добавляет в базу тестовые данные.
/// </summary>
/// <remarks>Этот сервис добавляется только в отладочные версии сервера приложения.</remarks>
[ApiController]
[Route("/api/telematica")]
[Consumes("application/json")]
[Produces("application/json")]
public class TestDataController : ValidationControllerBase
{
    private readonly ITrackerRepository _trackerRepository;
    private readonly ISensorTypeRepository _sensorTypeRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IConfiguration _configuration;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerTagRepository _trackerTagRepository;
    private readonly ILogger<TestDataController> _logger;
    private readonly ITrackerMessageRepository _trackerMessageRepository;

    private bool ServiceEnabled => _configuration["TestDataEnabled"] == "True";

    public TestDataController(ITrackerMessageRepository messageRepository,
        ITrackerRepository trackerRepository, ISensorTypeRepository sensorTypeRepository,
        IUnitRepository unitRepository, IConfiguration configuration, ITrackerTagRepository trackerTagRepository,
        ILogger<TestDataController> logger, ITrackerMessageRepository trackerMessageRepository)
    {
        _messageRepository = messageRepository;
        _trackerRepository = trackerRepository;
        _sensorTypeRepository = sensorTypeRepository;
        _unitRepository = unitRepository;
        _configuration = configuration;
        _trackerTagRepository = trackerTagRepository;
        _logger = logger;
        _trackerMessageRepository = trackerMessageRepository;
    }


    /// <summary>
    /// Генерирует и добавляет тестовые данные в базу данных. 
    /// </summary>
    /// <remarks>Это чисто отладочный метод. Поэтому тут нет обработки ошибок. В случае неудачи всегда возвращается статус 500</remarks>
    /// <response code="200">Данные успешно добавлены в базу</response>
    /// <response code="500">Внутренняя ошибка сервиса</response>
    [HttpPost("debug/add-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddTestData()
    {
        if (!ServiceEnabled)
        {
            return BadRequest("Test data service is not available!");
        }

        var sensorTypeIds = _sensorTypeRepository.GetSensorTypes().Select(v => v.Id);
        var unitIds = _unitRepository.GetUnits().Select(v => v.Id);
        var trackerTags = _trackerTagRepository.GetTags().ToArray();
        var trackers = Seeds.GenerateTrackers(sensorTypeIds.ToArray(), unitIds.ToArray(), trackerTags);
        foreach (var tracker in trackers)
        {
            if (tracker.Tracker == null)
                continue;
            _trackerRepository.Add(tracker.Tracker);
        }
        foreach (var message in trackers.Where(t => t.Messages != null).SelectMany(t => t.Messages!))
        {
            _trackerMessageRepository.Add(message);
        }
        return Ok();
    }

    /// <summary>
    /// Удаляет тестовые данные из базы. 
    /// </summary>
    /// <remarks>Тестовыми данными считаются все корневые сущности с отрицательными идентификаторами. Также будут удалены их
    /// дочерние сущности</remarks>
    /// <remarks>Это чисто отладочный метод. Поэтому тут нет обработки ошибок. В случае неудачи всегда возвращается статус 500</remarks>
    /// <response code="200">Данные удалены</response>
    /// <response code="500">Внутренняя ошибка сервиса</response>
    [HttpDelete("debug/delete-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteData()
    {
        if (!ServiceEnabled)
        {
            return BadRequest("Test data service is not available!");
        }

        var trackers = _trackerRepository.GetTrackers(new TrackersFilter()
        {
            PageSize = 100000
        });
        foreach (var tracker in trackers.Results)
        {
            if (tracker.Id >= 0)
                continue;
            _trackerRepository.Remove(tracker);
        }

        var messages = _messageRepository.GetMessages();
        foreach (var message in messages)
        {
            if (message.Id >= 0)
                continue;
            _messageRepository.Remove(message);
        }
        return Ok();
    }

    /// <summary>
    /// Тестовый макет расчета значений датчиков. 
    /// </summary>
    /// <remarks>Это чисто отладочный метод. Поэтому тут нет обработки ошибок. В случае неудачи всегда возвращается статус 500</remarks>
    /// <response code="200">Данные успешно добавлены в базу</response>
    /// <response code="500">Внутренняя ошибка сервиса</response>
    [HttpPost("debug/calc-sensors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CalcSensors()
    {
        if (!ServiceEnabled)
        {
            return BadRequest("Test data service is not available!");
        }

        var messages = _messageRepository.GetMessagesForUpdate();
        var pageWithTrackers = _trackerRepository.GetTrackers(new TrackersFilter
        {
            PageSize = 10000
        });
        for (var i = 0; i < messages.Count; i++)
        {
            var message = messages[i];
            MessageProcessing.CalculateFallBackValues(message, i > 0 ? messages[i - 1] : null);
        }
        var trackerTags = _trackerTagRepository.GetTags().ToArray();
        var exceptionHandler = new ExceptionHandler(_logger);
        messages.UpdateSensorTags(pageWithTrackers.Results, trackerTags, exceptionHandler);
        foreach (var message in messages)
        {
            _messageRepository.Update(message);
        }

        return Ok();
    }
}
