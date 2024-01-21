using BioTonFMS.Domain;
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
            .HasValue<MessageTagInteger>(TagDataTypeEnum.Integer)
            .HasValue<MessageTagBits>(TagDataTypeEnum.Bits)
            .HasValue<MessageTagByte>(TagDataTypeEnum.Byte)
            .HasValue<MessageTagDouble>(TagDataTypeEnum.Double)
            .HasValue<MessageTagBoolean>(TagDataTypeEnum.Boolean)
            .HasValue<MessageTagString>(TagDataTypeEnum.String)
            .HasValue<MessageTagDateTime>(TagDataTypeEnum.DateTime);

        modelBuilder.Entity<TrackerMessage>()
            .HasIndex("TrackerDateTime");

        modelBuilder.Entity<TrackerMessage>()
            .HasIndex("ExternalTrackerId");

        base.OnModelCreating(modelBuilder);
    }
}