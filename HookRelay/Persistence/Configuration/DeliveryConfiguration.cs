using HookRelay.Enums;
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
        builder.Property(d => d.EventId).IsRequired();
        builder.Property(d => d.WebhookId).IsRequired();
        builder.Property(d => d.Status).IsRequired();
        builder.Property(d => d.AttemptCount).IsRequired();
        builder.Property(d => d.NextRetryAt).IsRequired();
        builder.Property(d => d.LastError).IsRequired();
        builder.Property(d => d.CreatedAt).IsRequired();
        builder.Property(d => d.ProcessedAt).IsRequired();
        builder.HasOne(d => d.Event).WithMany(e => e.Deliveries).HasForeignKey(d => d.EventId);
        builder.HasOne(d => d.Webhook).WithMany().OnDelete(DeleteBehavior.Restrict);
    }
}