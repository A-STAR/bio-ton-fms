using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos
{
    public class RequestWithPaging
    {
        /// <summary>
        /// Номер страницы для постраничного вывода (начиная с 1)
        /// </summary>
        [Required]
        public int PageNum { get; set; }

        /// <summary>
        /// Размер страницы для постраничного вывода ( > 0) 
        /// </summary>
        [Required]
        public int PageSize { get; set; }
    }
}
