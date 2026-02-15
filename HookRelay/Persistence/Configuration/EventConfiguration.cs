using HookRelay.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HookRelay.Persistence.Configuration;

public class EventConfiguration:IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");
        builder.HasKey(e => e.EventId);
        builder.Property(e=> e.EventType).IsRequired();
        builder.Property(e=> e.Payload).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.HasIndex(e => e.EventType);
    }
}