namespace HookRelay.Persistence.Models;

public class Event
{
    public Guid EventId { get; private set; }
    public string EventType { get; private set; }

    public string Payload { get; private set; } // JSON string

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<Delivery> Deliveries { get; private set; } = new List<Delivery>();

    private Event(string eventType, string payload)
    {
        EventId = Guid.NewGuid();
        EventType = string.IsNullOrWhiteSpace(eventType) ? throw new ArgumentNullException("Event Type is required") : eventType;
        Payload = string.IsNullOrWhiteSpace(payload) ? throw new ArgumentNullException("Payload is required") : payload;
    }

    public static Event Create(string eventType, string payload) => new(eventType, payload);

}