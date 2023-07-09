using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Infrastructure.Extensions;
using System.ComponentModel.DataAnnotations;
using KeyValuePair = BioTonFMS.Infrastructure.Extensions.KeyValuePair;

namespace BioTonFMS.Telematica.Dtos.Monitoring;

/// <summary>
/// Модель машины
/// </summary>
public class MonitoringVehicleDto
{
    /// <summary>
    /// Id машины
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Наименование машины
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Идентификатор трекера во внешней системе
    /// </summary>
    public int? TrackerExternalId { get; set; }

    /// <summary>
    /// IMEI трекера
    /// </summary>
    public string? TrackerImei { get; set; } = "";


    /// <summary>
    /// Состояние движения
    /// </summary>
    public MovementStatusEnum MovementStatus { get; set; }

    /// <summary>
    /// Состояние соединения
    /// </summary>
    public ConnectionStatusEnum ConnectionStatus { get; set; }
        
    /// <summary>
    /// Количество спутников
    /// </summary>
    public int NumberOfSatellites { get; set; }

}

/// <summary>
/// Трекер
/// </summary>
public class VehicleTrackerDto
{
    /// <summary>
    /// Id трекера
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Наименование трекера
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
}