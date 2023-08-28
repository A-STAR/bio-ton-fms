using System.ComponentModel.DataAnnotations;
using BioTonFMS.Infrastructure.Models;
using CsvHelper.Configuration.Attributes;

namespace BioTonFMS.Domain.TrackerMessages;

public class TrackerMessageCsv
{
    /// <summary>
    /// id сообщения
    /// </summary>
    [Name("id")]
    public new long Id { get; set; }

    /// <summary>
    /// Уникальный идентификатор трекера 
    /// </summary>
    [Name("external_tracker_id")]
    public int ExternalTrackerId { get; set; }
    
    /// <summary>
    /// IMEI трекера
    /// </summary>
    [MaxLength(15)]
    [Name("imei")]
    public string Imei { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время регистрации сообщения на сервере
    /// </summary>
    [Name("server_date_time")]
    public DateTime ServerDateTime { get; set; }

    /// <summary>
    /// Дата и время регистрации сообщения на трекере
    /// </summary>
    [Name("tracker_date_time")] 
    public DateTime? TrackerDateTime { get; set; }

    /// <summary>
    /// Широта
    /// </summary>
    [Name("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Долгота
    /// </summary>

    [Name("longitude")]
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Число спутников
    /// </summary>
    [Name("sat_number")]
    public int? SatNumber { get; set; }

    /// <summary>
    /// Корректность координат
    /// </summary>
    [Name("coord_correctness")]
    public CoordCorrectnessEnum? CoordCorrectness { get; set; }

    /// <summary>
    /// Высота
    /// </summary>
    [Name("altitude")]
    public double? Altitude { get; set; }

    /// <summary>
    /// Скорость
    /// </summary>
    [Name("speed")] 
    public double? Speed { get; set; }
    
    /// <summary>
    /// Курс
    /// </summary>
    /// <returns></returns>
    [Name("direction")]
    public double? Direction { get; set; }

    /// <summary>
    /// Уровень топлива в %
    /// </summary>
    [Name("fuel_level")]
    public int? FuelLevel { get; set; }

    /// <summary>
    /// Температура охлаждающей жидкости
    /// </summary>
    [Name("coolant_temperature")]
    public int? CoolantTemperature { get; set; }

    /// <summary>
    /// Обороты двигателя
    /// </summary>
    [Name("engine_speed")]
    public int? EngineSpeed { get; set; }

    /// <summary>
    /// Идентификатор пакета данных трекера
    /// </summary>
    [Name("package_uid")]
    public Guid PackageUID { get; set; }
}