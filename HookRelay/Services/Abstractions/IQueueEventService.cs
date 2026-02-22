using HookRelay.Persistence.Models;

namespace HookRelay.Services.Abstractions;

public interface IQueueEventService
{
    public ValueTask EnqueueAsync(Event evt, CancellationToken ct = default);
}