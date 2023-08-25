using System.ComponentModel.DataAnnotations;
using BioTonFMS.Domain.Monitoring;

namespace BioTonFMS.Telematica.Dtos.Monitoring;

/// <summary>
/// Модель машины
/// </summary>
public class MonitoringVehicleDto
{
    /// <summary>
    /// Id машины
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Наименование машины
    /// </summary>
    [Required]
    public string Name { get; set; } = "";

    /// <summary>
    /// Трекер машины
    /// </summary>
    public MonitoringTrackerDto? Tracker { get; set; }

    /// <summary>
    /// Состояние движения
    /// </summary>
    public MovementStatusEnum MovementStatus { get; set; }

    /// <summary>
    /// Состояние соединения
    /// </summary>
    public ConnectionStatusEnum ConnectionStatus { get; set; }

    /// <summary>
    /// Время последнего сообщения трекера
    /// </summary>
    public DateTime? LastMessageTime { get; set; }

    /// <summary>
    /// Количество спутников
    /// </summary>
    public int? NumberOfSatellites { get; set; }

}
