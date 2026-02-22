using HookRelay.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Persistence.Repositories;

public class EventRepository(HookRelayDbContext dbContext)
{
    public async Task<bool> AddEventAsync(Event newEvent)
    {
            await dbContext.AddAsync(newEvent);
            return await dbContext.SaveChangesAsync() > 0;
    }
    public async Task<IEnumerable<Event>> ListAllEventsAsync()
    {
            return await dbContext.Events.AsNoTracking().ToListAsync();
    }
    public async Task<Event?> FindEventByIdAsync(Guid eventId)
    {
            return await dbContext.Events.FindAsync(eventId);
        
    }
}