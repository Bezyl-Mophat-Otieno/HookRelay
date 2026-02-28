using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class EventService(EventRepository eventRepository, IQueueEventService eventService):IEventService
{
    public async Task<Result<Event>> CreateEventAsync(Event evt, CancellationToken ct = default)
    {
        try
        {
          var saved =  await eventRepository.AddEventAsync(evt);
          if(!saved) return Result<Event>.Failure("New event entry failed to get saved please try again");
          await eventService.EnqueueAsync(evt, ct);
          return Result<Event>.Success(evt);
        }
        catch (Exception e)
        {
            return Result<Event>.Failure(e.Message);
        }
    }

    public async Task<Result<Event>> GetEventByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            if (ct.IsCancellationRequested) return Result<Event>.Failure("Event fetching request cancelled");
            var existingEvent = await eventRepository.FindEventByIdAsync(id);
            return existingEvent is null
                ? Result<Event>.Failure("Event not found")
                : Result<Event>.Success(existingEvent);
        }
        catch (Exception e)
        {
            return Result<Event>.Failure(e.Message);
        }
    }

    public async Task<Result<List<Event>>> ListAllEventAsync()
    {
        try
        {
            var events = await eventRepository.GetAllEvents();
            return Result<List<Event>>.Success(events);
        }
        catch (Exception e)
        {
            return Result<List<Event>>.Failure(e.Message);
        }        
    }
}