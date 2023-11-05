using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Telematica.Dtos.Monitoring;

namespace BioTonFMS.Telematica.Dtos.MessagesView;

public class MessagesViewTrackResponse
{
    /// <summary>
    /// Cтруктура содержащая координаты области карты для отображения на экране
    /// </summary>
    public ViewBounds? ViewBounds { get; set; }

    /// <summary>
    /// Данные для построения трека для выбранных сообщений
    /// </summary>
    public ICollection<TrackPointInfo> Track { get; set; } = new List<TrackPointInfo>();
}