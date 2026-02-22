using System.Text.Json;

namespace HookRelay.Dtos;

public record CreateEventRequest(
    string eventType,
    JsonElement payload
    );