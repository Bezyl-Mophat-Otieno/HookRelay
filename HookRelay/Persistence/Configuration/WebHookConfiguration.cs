using HookRelay.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HookRelay.Persistence.Configuration;

public class WebHookConfiguration:IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.ToTable("Webhooks");
        builder.HasKey(w => w.WebhookId);
        builder.Property(w => w.EventType).IsRequired();
        builder.Property(w => w.Secret).IsRequired();
        builder.Property(w => w.IsActive).IsRequired();
        builder.Property(w => w.CreatedAt).IsRequired();
        builder.HasIndex(w=> new {w.EventType, w.IsActive});
    }
}