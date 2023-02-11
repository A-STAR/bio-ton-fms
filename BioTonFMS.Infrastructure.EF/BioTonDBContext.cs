using BioTonFMS.Domain;
using BioTonFMS.Domain.Identity;
using BioTonFMS.Infrastructure.EF.Repositories.SensorGroups;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// ReSharper disable InconsistentNaming

namespace BioTonFMS.Infrastructure.EF
{
    public class BioTonDBContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public BioTonDBContext(DbContextOptions<BioTonDBContext> options)
            : base(options)
        {
        }

        public DbSet<Tracker> Trackers => Set<Tracker>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<TrackerTag> TrackerTags => Set<TrackerTag>();
        public DbSet<Sensor> Sensors => Set<Sensor>();
        public DbSet<SensorGroup> SensorGroups => Set<SensorGroup>();
        public DbSet<SensorType> SensorTypes => Set<SensorType>();
        public DbSet<Unit> Units => Set<Unit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tracker>()
              .HasIndex(u => u.ExternalId)
              .IsUnique();

            modelBuilder.Entity<TrackerTag>()
                .HasData(TagsSeed.TrackerTags);

            modelBuilder.Entity<ProtocolTag>()
                .HasData(TagsSeed.ProtocolTags);

            modelBuilder.Entity<SensorGroup>()
                .HasData(SensorGroupPredefinedData.SensorGroups);

            modelBuilder.Entity<SensorType>()
                .HasData(SensorTypePredefinedData.SensorTypes);

            modelBuilder.Entity<Unit>()
                .HasData(UnitPredefinedData.Units);
        }
    }
}
