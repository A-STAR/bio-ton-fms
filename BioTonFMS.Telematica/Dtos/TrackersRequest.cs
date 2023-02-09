using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Параметры выборки списка трекеров
    /// </summary>
    public class TrackersRequest : RequestWithPaging
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

        /// <summary>
        /// Сортировка по полю
        /// </summary>
        public TrackerSortBy? SortBy { get; set; }

        /// <summary>
        /// Направление сортировки
        /// </summary>
        public SortDirection? SortDirection { get; set; }
    }
}
