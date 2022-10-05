using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Domain
{
    public class Tracker : EntityBase, IAggregateRoot
    {
        public int? ExternalId { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [MaxLength(12)]
        public string SimNumber { get; set; }

        [Required]
        public TrackerTypeEnum TrackerType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }

        [Required]
        [MaxLength(30)]
        public string Imei { get; set; }

        public virtual IEnumerable<Device> Devices { get; set; }
    }
}