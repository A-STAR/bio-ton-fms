namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class TrackPointGeneralInfoDto
{
    /// <summary>
    /// Дата и время сообщения
    /// </summary>
    public DateTime? MessageTime { get; set; }
    
    /// <summary>
    /// Скорость из сообщения
    /// </summary>
    public double? Speed { get; set; }
    
    /// <summary>
    /// Количество спутников
    /// </summary>
    public int? NumberOfSatellites { get; set; }
    
    /// <summary>
    /// Широта
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Долгота
    /// </summary>
    public double? Longitude { get; set; }
}