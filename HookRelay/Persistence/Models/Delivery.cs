using HookRelay.Enums;

namespace HookRelay.Persistence.Models;

public class Delivery
{
    public Guid DeliveryId { get; private set; }
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null; // navigation property;

    public Guid WebhookId { get; private set; }
    public Webhook Webhook { get; private set; } = null; // navigation property;

    public DeliveryStatus Status { get; private set; } = DeliveryStatus.Pending;

    public int AttemptCount { get; private set; } = 0;

    public DateTime? NextRetryAt { get; private set; }

    public string? LastError { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; private set; }

    // private Delivery(Guid eventId, Guid webhookId, int attemptCount, DateTime, )
    // {
    //     
    // } 
    //
    // public static Delivery Create() => new();
}