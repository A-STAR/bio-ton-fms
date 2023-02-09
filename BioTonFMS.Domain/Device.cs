using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Domain
{
    public class Device : EntityBase
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = "";

        public int TrackerId { get; set; }

        public Tracker Tracker { get; set; } = null!;
    }
}