namespace BioTonFMS.Telematica.Dtos.Tracker
{
    /// <summary>
    /// Список трекеров с информацией о постраничном выводе
    /// </summary>
    public class TrackersResponse
    {
        /// <summary>
        /// Массив трекеров
        /// </summary>
        public TrackerDto[] Trackers { get; set; } = null!;

        /// <summary>
        /// Параметр постраничного вывода
        /// </summary>
        public Pagination Pagination { get; set; } = null!;
    }
}
