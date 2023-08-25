using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Параметры выборки списка датчиков
    /// </summary>
    public class SensorsRequest : RequestWithPaging
    {
        /// <summary>
        /// Идентификатор родительского трекера
        /// </summary>
        public int? TrackerId { get; set; }

        /// <summary>
        /// Сортировка по полю
        /// </summary>
        public SensorSortBy? SortBy { get; set; }

        /// <summary>
        /// Направление сортировки
        /// </summary>
        public SortDirection? SortDirection { get; set; }
    }
}
