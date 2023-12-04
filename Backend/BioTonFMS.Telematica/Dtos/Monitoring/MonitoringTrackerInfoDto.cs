using BioTonFMS.Domain.TrackerMessages;
using System.ComponentModel.DataAnnotations;
using BioTonFMS.Domain.MessagesView;

namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class MonitoringTrackerInfoDto
{
    [Required]
    public string TrackerType { get; set; } = "Galileo Sky v5.0";

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
    public IList<TrackerParameter> Parameters { get; set; } = new List<TrackerParameter>();

    /// <summary>
    /// Список датчиков и значения датчиков с соответствующими единицами измерения,
    /// которые рассчитаны по последнему пришедшему сообщению у которых признак “видимость” = TRUE.
    /// </summary>
    public IList<TrackerSensorDto> Sensors { get; set; } = new List<TrackerSensorDto>();
}