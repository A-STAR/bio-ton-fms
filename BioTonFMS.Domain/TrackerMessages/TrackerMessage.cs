using System.ComponentModel.DataAnnotations;
using BioTonFMS.Infrastructure.Models;

namespace BioTonFMS.Domain.TrackerMessages;

public class TrackerMessage : EntityBase, IAggregateRoot
{
    /// <summary>
    /// id сообщения
    /// </summary>
    public new long Id { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор трекера
    /// </summary>
    public int TrId { get; set; }
    
    /// <summary>
    /// IMEI трекера
    /// </summary>
    [MaxLength(15)]
    public string Imei { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата и время регистрации сообщения на сервере
    /// </summary>
    public DateTime ServerDateTime { get; set; }
    
    /// <summary>
    /// Дата и время регистрации сообщения на трекере
    /// </summary>
    public DateTime? TrackerDateTime { get; set; }
    
    /// <summary>
    /// Широта
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Долгота
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Число спутников
    /// </summary>
    public int? SatNumber { get; set; }
    
    /// <summary>
    /// Корректность координат
    /// </summary>
    public CoordCorrectnessEnum? CoordCorrectness { get; set; }
    
    /// <summary>
    /// Высота
    /// </summary>
    public double? Height { get; set; }
    
    /// <summary>
    /// Скорость
    /// </summary>
    public double? Speed { get; set; }
    
    /// <summary>
    /// Курс
    /// </summary>
    /// <returns></returns>
    public double? Direction { get; set; }
    
    /// <summary>
    /// Уровень топлива в %
    /// </summary>
    public int? FuelLevel { get; set; }
    
    /// <summary>
    /// Температура охлаждающей жидкости
    /// </summary>
    public int? CoolantTemperature { get; set; }

    /// <summary>
    /// Обороты двигателя
    /// </summary>
    public int? EngineSpeed { get; set; }

    /// <summary>
    /// Набор тегов для сообщения
    /// </summary>
    public IList<MessageTag> Tags { get; set; } = new List<MessageTag>();
}