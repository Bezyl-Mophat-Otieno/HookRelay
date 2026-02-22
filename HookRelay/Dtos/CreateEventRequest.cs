namespace HookRelay.Dtos;

public record CreateEventRequest(
    string eventType,
    string payload
    );