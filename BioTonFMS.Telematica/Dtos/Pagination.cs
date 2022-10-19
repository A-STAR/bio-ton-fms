namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Параметры постраничного вывода
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Номер страницы для постраничного вывода
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Размер страницы для постраничного вывода
        /// </summary>
        public int Total { get; set; }
    }
}
