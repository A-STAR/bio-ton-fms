using BioTonFMS.Domain;
using BioTonFMS.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF
{
    public class BioTonDBContext : IdentityDbContext<AppUser, AppRole, int>
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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>().HasData(
             new Vehicle { Id = 1, Name = "vehicle 1", TrackerId = 1 },
             new Vehicle { Id = 2, Name = "vehicle 2", TrackerId = 2 });

            modelBuilder.Entity<Tracker>()
                .HasIndex(u => u.ExternalId)
                .IsUnique();

            modelBuilder.Entity<Tracker>().HasData(
             new Tracker
             {
                 Id = 1,
                 Name = "tracker 2",
                 Description = "tracker_description 1",
                 Imei = "12345678",
                 SimNumber = "123456789121",
                 StartDate = DateTime.UtcNow,
                 TrackerType = TrackerTypeEnum.Retranslator
             },
             new Tracker
             {
                 Id = 2,
                 Name = "tracker 2",
                 Description = "tracker_description 2",
                 Imei = "22345679",
                 SimNumber = "12345678912",
                 StartDate = DateTime.UtcNow,
                 TrackerType = TrackerTypeEnum.Retranslator
             });

            modelBuilder.Entity<Device>().HasData(
                new Device { Id = 1, Name = "D1", TrackerId = 2 },
                new Device { Id = 2, Name = "D2", TrackerId = 2 },
                new Device { Id = 3, Name = "D3", TrackerId = 2 });
        }
    }
}