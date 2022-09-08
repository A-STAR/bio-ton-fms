using BioTonFMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF
{
    public class BioTonDBContext : DbContext
    {
        public BioTonDBContext(DbContextOptions<BioTonDBContext> options)
            : base(options)
        {
        }

        public DbSet<Tracker> Trackers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Device> Devices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}