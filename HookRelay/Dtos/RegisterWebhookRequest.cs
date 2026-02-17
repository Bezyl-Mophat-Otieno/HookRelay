namespace HookRelay.Dtos;

public record RegisterWebhookRequest(
    string url,
    string eventType,
    string secret,
    bool? isActive 
    );