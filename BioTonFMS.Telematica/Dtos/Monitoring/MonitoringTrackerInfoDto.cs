using BioTonFMS.Domain.TrackerMessages;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class MonitoringTrackerInfoDto
{
    [Required]
    public string TrackerType { get; set; }

    /// <summary>
    /// Внешний ID трекера
    /// </summary>
    [Required]
    public int ExternalId { get; set; }

    /// <summary>
    /// IMEI трекера
    /// </summary>
    [Required]
    public string Imei { get; set; }

    /// <summary>
    /// Номер сим карты
    /// </summary>
    [Required]
    public string SimNumber { get; set; }

    /// <summary>
    /// Параметры трекера
    /// </summary>
    public TrackerParameter[] Parameters { get; set; }

    /// <summary>
    /// Список датчиков и значения датчиков с соответствующими единицами измерения,
    /// которые рассчитаны по последнему пришедшему сообщению у которых признак “видимость” = TRUE.
    /// </summary>
    public TrackerSensorDto[] Sensors { get; set; }
}