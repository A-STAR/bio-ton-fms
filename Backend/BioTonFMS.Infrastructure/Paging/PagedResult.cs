namespace BioTonFMS.Infrastructure.Paging
{
    public class PagedResult<T> where T : class
    {
        // Текущая страница
        public int CurrentPage { get; set; }

        // Общее количество страниц в результате запроса
        public int TotalPageCount { get; set; }

        // Количество элементов на странице
        public int PageSize { get; set; }

        // Общее количество элементов в результате запроса
        public int TotalRowCount { get; set; }

        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}
