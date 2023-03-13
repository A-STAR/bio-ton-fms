using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BioTonFMS.Infrastructure.EF.Models.Filters
{
    public class TrackersFilter : SortableFilterWithPaging<TrackerSortBy?>
    {
        /// <summary>
        /// Идентификатор трекера
        /// </summary>
        public int? Id { get; set; }
        
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
