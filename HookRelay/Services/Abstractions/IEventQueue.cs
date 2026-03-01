using HookRelay.Persistence.Models;

namespace HookRelay.Services.Abstractions;

public interface IEventQueue
{
    public ValueTask EnqueueAsync(Event evt, CancellationToken ct = default);
}