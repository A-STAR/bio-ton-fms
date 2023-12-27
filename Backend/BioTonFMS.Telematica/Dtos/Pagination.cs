namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Параметры постраничного вывода
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Общее количество страниц для вывода всех данных
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Общее количество записей
        /// </summary>
        public int Records { get; set; }
    }
}
