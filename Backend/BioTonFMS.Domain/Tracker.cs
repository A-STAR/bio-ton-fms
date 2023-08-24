using BioTonFMS.Common.Testable;
using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace BioTonFMS.Domain;

/// <summary>
/// Трекер
/// </summary>
public class Tracker : EntityBase, IAggregateRoot
{
    /// <summary>
    /// Идентификатор трекера во внешней системе
    /// </summary>
    [Required]
    public int ExternalId { get; set; }

    /// <summary>
    /// Название трекера
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    /// <summary>
    /// Номер sim – карты
    /// </summary>
    [Required]
    [MaxLength(12)]
    public string SimNumber { get; set; } = "";

    /// <summary>
    /// Тип устройства
    /// </summary>
    [Required]
    public TrackerTypeEnum TrackerType { get; set; }

    /// <summary>
    /// Дата и время начала действия трекера на данной машине
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = "";

    /// <summary>
    /// IMEI трекера
    /// </summary>
    [Required]
    [MaxLength(15)]
    public string Imei { get; set; } = "";

    /// <summary>
    /// IP адрес трекера
    /// </summary>
    [MaxLength(15)]
    public string? IpAddress { get; set; }
        
    /// <summary>
    /// Порт трекера
    /// </summary>
    public int? Port { get; set; }
        
    /// <summary>
    /// Дата и время когда было принято последнее сообщение
    /// </summary>
    public DateTime? LastMessageReceived { get; set; }

    /// <summary>
    /// Машина
    /// </summary>
    public Vehicle? Vehicle { get; set; }

    /// <summary>
    /// Датчики
    /// </summary>
    public List<Sensor> Sensors { get; set; } = new();

    /// <summary>
    /// Датчики
    /// </summary>
    public List<TrackerCommand> Commands { get; set; } = new();

    public void SetTrackerAddress(string ipAddress, int port)
    {
        IpAddress = ipAddress;
        Port = port;
        LastMessageReceived = SystemTime.UtcNow;
    }
}