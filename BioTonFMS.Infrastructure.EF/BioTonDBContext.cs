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
        }
    }
}