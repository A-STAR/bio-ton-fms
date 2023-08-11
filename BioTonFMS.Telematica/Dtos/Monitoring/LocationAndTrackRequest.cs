namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class LocationAndTrackRequest
{
    /// <summary>
    /// Идентификатор машины
    /// </summary>
    public int VehicleId { get; set; }

    /// <summary>
    /// Признак, нужно ли возвращать данные о треке для текущей машины
    /// </summary>
    public bool NeedReturnTrack { get; set; }
}