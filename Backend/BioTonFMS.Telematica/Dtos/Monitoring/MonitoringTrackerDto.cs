namespace BioTonFMS.Telematica.Dtos.Monitoring;

/// <summary>
/// Модель трекера
/// </summary>
public class MonitoringTrackerDto
{
    /// <summary>
    /// Id трекера
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// ExternalId трекера
    /// </summary>
    public int? ExternalId { get; set; }
    
    /// <summary>
    /// IMEI трекера
    /// </summary>
    public string? Imei { get; set; }
}