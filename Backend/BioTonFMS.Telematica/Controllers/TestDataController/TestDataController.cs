using AutoMapper;
using BioTonFMS.Domain.Identity;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Controllers;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using BioTonFMS.Infrastructure.EF.Repositories.VehicleGroups;
using BioTonFMS.Infrastructure.EF.Repositories.Vehicles;
using BioTonFMS.MessageProcessing;
using BioTonFMS.Telematica.Data.Mapping;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using BioTonFMS.Common.Extensions;
using BioTonFMS.Telematica.Services;

namespace BioTonFMS.Telematica.Controllers.TestDataController;

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
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ISensorTypeRepository _sensorTypeRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IConfiguration _configuration;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ITrackerTagRepository _trackerTagRepository;
    private readonly ILogger<TestDataController> _logger;
    private readonly ITrackerMessageRepository _trackerMessageRepository;
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IFuelTypeRepository _fuelTypeRepository;
    private readonly MoveTestTrackerMessagesService _moveTestTrackerMessagesService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    private bool ServiceEnabled => _configuration["TestDataEnabled"] == "True";

    public TestDataController(
        ILogger<TestDataController> logger, UserManager<AppUser> userManager, IConfiguration configuration,
        ITrackerMessageRepository messageRepository,
        ITrackerRepository trackerRepository, ISensorTypeRepository sensorTypeRepository,
        IVehicleRepository vehicleRepository, IUnitRepository unitRepository,
        ITrackerTagRepository trackerTagRepository, IVehicleGroupRepository vehicleGroupRepository,
        ITrackerMessageRepository trackerMessageRepository, IFuelTypeRepository fuelTypeRepository,
        MoveTestTrackerMessagesService moveTestTrackerMessagesService,
        IMapper mapper)
    {
        _logger = logger;
        _userManager = userManager;
        _configuration = configuration;
        _messageRepository = messageRepository;
        _trackerRepository = trackerRepository;
        _vehicleRepository = vehicleRepository;
        _sensorTypeRepository = sensorTypeRepository;
        _unitRepository = unitRepository;
        _trackerTagRepository = trackerTagRepository;
        _trackerMessageRepository = trackerMessageRepository;
        _vehicleGroupRepository = vehicleGroupRepository;
        _fuelTypeRepository = fuelTypeRepository;
        _moveTestTrackerMessagesService = moveTestTrackerMessagesService;
        _mapper = mapper;
    }

    /// <summary>
    /// Добавляет тестового пользоветеля test/test в базу данных. 
    /// </summary>
    /// <remarks>Это чисто отладочный метод. Поэтому тут нет обработки ошибок. В случае неудачи всегда возвращается статус 500</remarks>
    /// <response code="200">Данные успешно добавлены в базу</response>
    /// <response code="500">Внутренняя ошибка сервиса</response>
    [HttpPost("debug/add-test-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddTestUser()
    {
        if (!ServiceEnabled)
        {
            return BadRequest("Test data service is not available!");
        }
        var user = new AppUser
        {
            FirstName = "Иван", LastName = "Тестов", UserName = "test",
        };

        var result = await _userManager.CreateAsync(user, "test");
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
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
        // удаляем старые тестовые данные, чтобы избежать конфликтов 
        DeleteData();
        var sensorTypeIds = _sensorTypeRepository.GetSensorTypes().Select(v => v.Id);
        var unitIds = _unitRepository.GetUnits().Select(v => v.Id);
        var trackerTags = _trackerTagRepository.GetTags().ToArray();
        var trackerDatas = Seeds.GenerateTrackers(sensorTypeIds.ToArray(), unitIds.ToArray(), trackerTags);
        foreach (var trackerData in trackerDatas)
        {
            if (trackerData.Tracker == null)
                continue;
            _trackerRepository.Add(trackerData.Tracker);
        }
        /*foreach (var message in trackerDatas.Where(t => t.Messages is not null).SelectMany(t => t.Messages!))
        {
            _trackerMessageRepository.Add(message);
        }*/

        var vehicleGroupIds = _vehicleGroupRepository.GetVehicleGroups().Select(v => v.Id);
        var fuelTypeIds = _fuelTypeRepository.GetFuelTypes().Select(v => v.Id);
        var vehicles = Seeds.GenerateVehicles(trackerDatas.Where(td => td.Tracker is not null)
            .Select(td => td.Tracker!).ToArray(), vehicleGroupIds.ToArray(), fuelTypeIds.ToArray());
        foreach (var vehicle in vehicles)
        {
            _vehicleRepository.Add(vehicle);
        }

        return Ok();
    }

    /// <summary>
    /// Добавляет тестовые данные треков на сегодняшний день. 
    /// </summary>
    /// <remarks>Это чисто отладочный метод. Поэтому тут нет обработки ошибок. В случае неудачи всегда возвращается статус 500</remarks>
    /// <response code="200">Данные успешно добавлены в базу</response>
    /// <response code="500">Внутренняя ошибка сервиса</response>
    [HttpPost("debug/add-tracks-for-today/{limit:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddTestTrackDataForToday(int limit = 10)
    {
        var dataFiles = new List<(string MessageFile, string TagsFile)>()
        {
            ("1734-messages.csv", "1734-message-tags.csv"),
            ("1822-messages.csv", "1822-message-tags.csv"),
            ("1827-messages.csv", "1827-message-tags.csv"),
            ("1834-messages.csv", "1834-message-tags.csv"),
            ("2102-messages.csv", "2102-message-tags.csv"),
            ("2703-messages.csv", "2703-message-tags.csv"),
            ("2714-messages.csv", "2714-message-tags.csv"),
            ("2717-messages.csv", "2717-message-tags.csv"),
            ("3035-messages.csv", "3035-message-tags.csv")

        }.ToArray();

        var trackers = _trackerRepository.GetTrackers(new TrackersFilter()
        {
            PageSize = 100000
        }, forUpdate: true);

        var dataFileIndex = 0;
        foreach (var tracker in trackers.Results)
        {
            if (tracker.Id >= 0)
                continue;
            if (dataFileIndex > limit - 1)
                continue;
            // Удаляем сообщения за текущий день
            var todayMessages = _messageRepository.GetTrackerMessagesForDate(new int[] { tracker.ExternalId }, DateOnly.FromDateTime(DateTime.Today), false);
            foreach (var message in todayMessages)
            { 
                _messageRepository.Remove(message);
            }

            if (dataFileIndex < dataFiles.Length -1)
            {
                var dataFile = dataFiles[dataFileIndex];

                MemoryStream stream = new MemoryStream();

                var type = GetType();
                var messageFileName = @"BioTonFMS.Telematica.Data.TrackerMessages." + dataFile.MessageFile;
                var tagsFileName = @"BioTonFMS.Telematica.Data.TrackerMessages." + dataFile.TagsFile;

                var resources = type.Assembly.GetManifestResourceNames();

                using (var messageFileStreamReader = new StreamReader(type.Assembly.GetManifestResourceStream(messageFileName)))
                using (var tagsFileStreamReader = new StreamReader(type.Assembly.GetManifestResourceStream(tagsFileName)))
                using (var messageCsv = new CsvReader(messageFileStreamReader, CultureInfo.InvariantCulture))
                using (var tagsCsv = new CsvReader(tagsFileStreamReader, CultureInfo.InvariantCulture))
                {
                    SetNullableOptions(messageCsv);
                    SetNullableOptions(tagsCsv);

                    var csvMessages = messageCsv.GetRecords<TrackerMessageCsv>().ToArray();
                    var csvTagsForMessages = tagsCsv.GetRecords<MessageTagCsv>().ToArray();
                    foreach (var csvMessage in csvMessages)
                    {
                        var message = _mapper.Map<TrackerMessage>(csvMessage);

                        message.ServerDateTime = message.ServerDateTime.ToToday();
                        if (message.TrackerDateTime is not null)
                        {
                            message.TrackerDateTime = message.TrackerDateTime.Value.ToToday();
                        }
                        message.ExternalTrackerId = tracker.ExternalId;
                        message.Imei = tracker.Imei;
                        var csvMessageTags = csvTagsForMessages.Where(t => t.TrackerMessageId == csvMessage.Id);
                        var messageTags = csvMessageTags.ConvertToMessageTags(message);
                        message.Tags = messageTags;
                        _trackerMessageRepository.Add(message);
                    }
                }

                dataFileIndex++;
            }
        }

        return Ok();
    }

    private static void SetNullableOptions(CsvReader messageCsv)
    {
        messageCsv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>().NullValues.Add("NULL");
        messageCsv.Context.TypeConverterOptionsCache.GetOptions<double?>().NullValues.Add("NULL");
        messageCsv.Context.TypeConverterOptionsCache.GetOptions<int?>().NullValues.Add("NULL");
        messageCsv.Context.TypeConverterOptionsCache.GetOptions<CoordCorrectnessEnum?>().NullValues.Add("NULL");
        messageCsv.Context.TypeConverterOptionsCache.GetOptions<byte?>().NullValues.Add("NULL");
        messageCsv.Context.TypeConverterOptionsCache.GetOptions<bool?>().NullValues.Add("NULL");
    }

    /// <summary>
    /// Сдвигает тестовые данные треков на сегодняшний день. 
    /// </summary>
    /// <remarks>Это чисто отладочный метод. Поэтому тут нет обработки ошибок. В случае неудачи всегда возвращается статус 500</remarks>
    /// <response code="200">Данные успешно обновлены</response>
    /// <response code="500">Внутренняя ошибка сервиса</response>
    [HttpPost("debug/move-test-messages-to-today/{fromDate}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult MoveTestMessagesToToday(DateTime fromDate)
    {
        _moveTestTrackerMessagesService.MoveTestTrackerMessagesForToday(DateOnly.FromDateTime(fromDate.Add(fromDate.TimeOfDay)));

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

        var vehicles = _vehicleRepository.GetVehicles(new VehiclesFilter()
        {
            PageSize = 100000
        }, hydrate: false);
        foreach (var vehicle in vehicles.Results)
        {
            if (vehicle.Id >= 0)
                continue;
            _vehicleRepository.Remove(vehicle);
        }

        var trackers = _trackerRepository.GetTrackers(new TrackersFilter()
        {
            PageSize = 100000
        }, forUpdate: true);
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

        var messages = _messageRepository.GetMessages(forUpdate: true);
        var pageWithTrackers = _trackerRepository.GetTrackers(new TrackersFilter
        {
            PageSize = 10000
        });
        for (var i = 0; i < messages.Count; i++)
        {
            var message = messages[i];
            MessageProcessing.MessageProcessing.CalculateFallBackValues(message, i > 0 ? messages[i - 1] : null);
        }
        var trackerTags = _trackerTagRepository.GetTags().ToArray();
        var exceptionHandler = new LoggingExceptionHandler(_logger);
        messages.UpdateSensorTags(previousMessages: new Dictionary<int, TrackerMessage>(), pageWithTrackers.Results, trackerTags,
            exceptionHandler);

        foreach (var message in messages)
        {
            _messageRepository.Update(message);
        }

        return Ok();
    }
}
