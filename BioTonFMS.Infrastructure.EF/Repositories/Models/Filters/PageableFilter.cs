using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models;

namespace BioTonFMS.Infrastructure.EF.Models.Filters
{
    public abstract class PageableFilter
    {
        /// <summary>
        /// Номер страницы для постраничного вывода
        /// </summary>
        public int PageNum { get; set; } = 1;

        /// <summary>
        /// Размер страницы для постраничного вывода
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
