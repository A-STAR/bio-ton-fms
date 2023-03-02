using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models.Filters;

namespace BioTonFMS.Infrastructure.EF.Repositories.Models.Filters
{
    public class TrackersFilter : SortableFilterWithPaging<TrackerSortBy?>
    {
        /// <summary>
        /// Внешний идентификатор
        /// </summary>
        public int? ExternalId { get; set; }

        /// <summary>
        /// Номер сим карты
        /// </summary>
        public string? SimNumber { get; set; }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public TrackerTypeEnum? Type { get; set; }
    }
}
