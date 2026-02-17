namespace HookRelay.Persistence.Models;

public class Webhook
{
    public Guid WebhookId { get; private set; }
    
    public string Url { get; private set; }

    public string EventType { get; private set; }

    public string Secret { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;   
    
    private Webhook()
    {
        
    }
    
    private Webhook(string url, string eventType, string secret)
    {
        WebhookId = Guid.NewGuid();
        Url = string.IsNullOrWhiteSpace(url) ? throw new ArgumentException("Url required") : url;
        EventType = string.IsNullOrWhiteSpace(eventType) ? throw new ArgumentException("EventType required") : eventType;
        Secret = secret ?? throw new ArgumentNullException(nameof(secret));
    }

    public static Webhook Create(string url, string eventType, string secret) => new(url, eventType, secret);
}