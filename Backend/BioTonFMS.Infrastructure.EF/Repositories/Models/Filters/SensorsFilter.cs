using BioTonFMS.Infrastructure.EF.Repositories.Models;

namespace BioTonFMS.Infrastructure.EF.Models.Filters
{
    public class SensorsFilter : SortableFilterWithPaging<SensorSortBy?>
    {
        /// <summary>
        /// Родительский трекер
        /// </summary>
        public int? TrackerId { get; set; }
    }
}
