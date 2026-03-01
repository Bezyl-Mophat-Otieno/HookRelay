using HookRelay.Persistence.Models;

namespace HookRelay.Services.Abstractions;

public interface IEventProcessor
{
   public Task ProcessEventAsync(Event evt, CancellationToken ct);
}