using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.Sensors;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BioTonFMS.Telematica.Controllers;

public static class Seeds
{
    public static List<Tracker> GenerateTrackers(int[] sensorTypeIds, int[] unitIds)
    {
        var sensorId = -1;
        var sensors = new Faker<Sensor>()
            .RuleFor(v => v.Id, (f, v) => sensorId--)
            .RuleFor(v => v.Name, (f, v) => f.Hacker.Noun())
            .RuleFor(v => v.Formula, (f, v) => "someParam")
            .RuleFor(v => v.SensorTypeId, (f, v) => sensorTypeIds[f.Random.Int(0, sensorTypeIds.Length - 1)])
            .RuleFor(v => v.UnitId, (f, v) => unitIds[f.Random.Int(0, unitIds.Length - 1)]);

        var trackerId = -1;
        var trackers = new Faker<Tracker>()
            .RuleFor(v => v.Id, (f, v) => trackerId--)
            .RuleFor(v => v.Name, (f, v) => f.Hacker.Adjective())
            .RuleFor(v => v.Sensors, (f, v) =>
            {
                var result = sensors.Generate(10);
                result.ForEach(s =>
                {
                    s.TrackerId = v.Id;
                });
                return result;
            })
            .RuleFor(v => v.ExternalId, (f, v) => v.Id);

        return trackers.Generate(10);
    }
}
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
    private readonly ISensorRepository _sensorRepository;
    private readonly ITrackerRepository _trackerRepository;
    private readonly ISensorTypeRepository _sensorTypeRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IConfiguration _configuration;
    
    private bool ServiceEnabled => _configuration["TestDataEnabled"] == "True";

    public TestDataController(
        ISensorRepository sensorRepository, ITrackerRepository trackerRepository, ISensorTypeRepository sensorTypeRepository,
        IUnitRepository unitRepository, IConfiguration configuration)
    {
        _sensorRepository = sensorRepository;
        _trackerRepository = trackerRepository;
        _sensorTypeRepository = sensorTypeRepository;
        _unitRepository = unitRepository;
        _configuration = configuration;
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
        var trackers = Seeds.GenerateTrackers(sensorTypeIds.ToArray(), unitIds.ToArray());
        foreach (var tracker in trackers)
        {
            _trackerRepository.Add(tracker);
        }
        foreach (var sensor in trackers.SelectMany(t => t.Sensors))
        {
            _sensorRepository.Add(sensor);
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
        return Ok();
    }
}
