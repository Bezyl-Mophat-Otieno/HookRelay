using System.Threading.Channels;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Services;

public class EventDispatcherWorker(Channel<Event>channel, IServiceScopeFactory scopeFactory, ILogger<EventDispatcherWorker> logger):BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            await foreach (var evt in channel.Reader.ReadAllAsync(stoppingToken))
            {
                logger.LogInformation("EventDispatcherWorker started.");
                try
                {
                    logger.LogInformation("Processing Event {EventType} Created on {CreatedAt:yy-MMM-dd ddd}", evt.EventType, evt.CreatedAt);
                    await ProcessEvent(evt, stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error processing event {EventId}", evt.EventId);
                }
            }
        }
            
    }

    private async Task ProcessEvent(Event evt, CancellationToken token)
    {
        var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HookRelayDbContext>();
        var webhooks = await db.Webhooks.Where(whk => whk.EventType == evt.EventType).ToListAsync(token);
        logger.LogInformation("Dispatching {Count} webhooks for {EventId}", webhooks.Count, evt.EventId);
        // Next step : Process Delivery
    }
}