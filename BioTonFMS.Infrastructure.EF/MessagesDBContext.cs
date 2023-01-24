using BioTonFMS.Domain.TrackerMessages;
using Microsoft.EntityFrameworkCore;

// ReSharper disable InconsistentNaming

namespace BioTonFMS.Infrastructure.EF;

public class MessagesDBContext : DbContext
{
    public MessagesDBContext(DbContextOptions<MessagesDBContext> options)
        : base(options)
    {
    }
    
    public DbSet<TrackerMessage> Trackers => Set<TrackerMessage>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}