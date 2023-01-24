using BioTonFMS.Domain;
using BioTonFMS.Domain.Identity;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using Bogus;
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
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<TrackerTag> TrackerTags => Set<TrackerTag>();
        public DbSet<Sensor> Sensors => Set<Sensor>();

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

            Randomizer.Seed = new Random(8675309);
            var trackers = Seeds.GenerateTrackers();
            modelBuilder.Entity<Tracker>()
                .HasData(trackers);

            modelBuilder.Entity<Sensor>()
                .HasData(trackers.Aggregate(new List<Sensor>(10), (list, tracker) =>
                {
                    list.AddRange(tracker.Sensors);
                    return list;
                }));
        }
    }
}
