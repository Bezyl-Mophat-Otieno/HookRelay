using HookRelay.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HookRelay.Persistence.Configuration;

public class DeliveryConfiguration:IEntityTypeConfiguration<Delivery>
{
    private IEntityTypeConfiguration<Delivery> _entityTypeConfigurationImplementation;
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("Deliveries");
        builder.HasKey(d=> d.DeliveryId);
        builder.Property(d => d.EventId);
        builder.Property(d => d.WebhookId);
        builder.Property(d => d.Status);
        builder.Property(d => d.AttemptCount);
        builder.Property(d => d.NextRetryAt);
        builder.Property(d => d.LastError);
        builder.Property(d => d.CreatedAt);
        builder.Property(d => d.ProcessedAt);
        builder.HasOne(d => d.Event).WithMany(e => e.Deliveries).HasForeignKey(d => d.EventId);
        builder.HasOne(d => d.Webhook);
    }
}