namespace BioTonFMS.Infrastructure.EF.Models.Filters
{
    public abstract class SortableFilterWithPaging<SortByEnum> : PageableFilter
    {
        /// <summary>
        /// Сортировка по полю
        /// </summary>
        public SortByEnum SortBy { get; set; }

        /// <summary>
        /// Направление сортировки
        /// </summary>
        public SortDirection? SortDirection { get; set; }
    }
}
