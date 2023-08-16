namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class LocationsAndTracksResponse
{
    /// <summary>
    /// Cтруктура содержащая координаты области карты для отображения на экране
    /// </summary>
    public ViewBounds? ViewBounds { get; set; }

    /// <summary>
    /// Данные для построения быстрого трека для заданных машин
    /// </summary>
    public ICollection<LocationAndTrack> Tracks { get; set; }
}