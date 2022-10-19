namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Список трекеров с информацией о постраничном выводе
    /// </summary>
    public class TrackersResponse
    {
        /// <summary>
        /// Массив трекеров
        /// </summary>
        public TrackerDto[] Trackers { get; set; }

        /// <summary>
        /// Параметр постраничного вывода
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}
