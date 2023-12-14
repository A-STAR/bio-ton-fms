using BioTonFMS.Domain.Monitoring;

namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class LocationAndTrack
{
    /// <summary>
    /// Идентификатор машины
    /// </summary>
    public int VehicleId { get; set; }

    /// <summary>
    /// Имя машины
    /// </summary>
    public string VehicleName { get; set; }

    /// <summary>
    /// Широта положения машины из последнего сообщения, полученного от её трекера
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Долгота положения машины из последнего сообщения, полученного от её трекера
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Содержит данные трека для машины за последние сутки, если для этой машины был запрошен трек.
    /// Если трек не был запрошен возвращается пустой массив.
    /// </summary>
    public TrackPointInfo[] Track { get; set; } = Array.Empty<TrackPointInfo>();
}