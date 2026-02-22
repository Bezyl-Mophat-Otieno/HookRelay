using HookRelay.Persistence;
using HookRelay.Persistence.Models;

namespace HookRelay.Services.Abstractions;

public interface IEventService
{
    public Task<Result<Event>> CreateEventAsync(Event evt, CancellationToken ct=default);
    public Task<Result<Event>> GetEventByIdAsync(Guid id, CancellationToken ct=default);
    
    
}