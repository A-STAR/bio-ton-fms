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

        public DbSet<Tracker> Trackers => Set<Tracker>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Device> Devices => Set<Device>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tracker>()
              .HasIndex(u => u.ExternalId)
              .IsUnique();
        }
    }
}