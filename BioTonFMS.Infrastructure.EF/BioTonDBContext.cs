using BioTonFMS.Domain;
using BioTonFMS.Domain.Identity;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
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

        public DbSet<Tracker> Trackers => Set<Tracker>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<TrackerTag> TrackerTags => Set<TrackerTag>();

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
        }
    }
}