using BioTonFMS.Domain.TrackerMessages;

namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class TrackPointTrackerInfoDto
{
    /// <summary>
    /// Параметры трекера из заданного сообщения
    /// </summary>
    public ICollection<TrackerParameter> Parameters { get; set; } = new List<TrackerParameter>();

    /// <summary>
    /// Список датчиков и значения датчиков с соответствующими единицами измерения,
    /// которые рассчитаны по заданному сообщению у которых признак "видимость" = TRUE.
    /// </summary>
    public ICollection<TrackerSensorDto> Sensors { get; set; } = new List<TrackerSensorDto>();
}