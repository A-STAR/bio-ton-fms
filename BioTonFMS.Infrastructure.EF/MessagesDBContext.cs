using BioTonFMS.Domain.TrackerMessages;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF;

public class MessagesDBContext : DbContext 
{
    public MessagesDBContext(DbContextOptions<MessagesDBContext> options)
        : base(options)
    {
    }
    
    public DbSet<TrackerMessage> TrackerMessages => Set<TrackerMessage>();
    public DbSet<MessageTag> MessageTags => Set<MessageTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageTag>()
            .HasDiscriminator(x => x.TagType)
            .HasValue<MessageTagInteger>(1)
            .HasValue<MessageTagBits>(2)
            .HasValue<MessageTagByte>(3)
            .HasValue<MessageTagDouble>(4)
            .HasValue<MessageTagBoolean>(5)
            .HasValue<MessageTagString>(6)
            .HasValue<MessageTagDateTime>(7);
        
        base.OnModelCreating(modelBuilder);
    }
}