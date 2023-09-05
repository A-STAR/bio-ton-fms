namespace BioTonFMS.Domain.Monitoring;

public class TrackPointInfo
{
    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public long MessageId { get; set; }

    /// <summary>
    /// Дата и время получения сообщения
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// Широта точки трека
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Долгота точки трека
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Скорость в точке (может быть пустым)
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Количество спутников
    /// </summary>
    public int? NumberOfSatellites { get; set; }
}