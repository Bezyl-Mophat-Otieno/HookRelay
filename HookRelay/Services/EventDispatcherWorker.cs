using System.Threading.Channels;
using HookRelay.Persistence.Models;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class EventDispatcherWorker(Channel<Event>eventsChannel, IServiceScopeFactory scopeFactory, ILogger<EventDispatcherWorker> logger, IHttpClientFactory httpClientFactory):BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("EventDispatcherWorker running...");
            await foreach (var evt in eventsChannel.Reader.ReadAllAsync(stoppingToken))
            {
                var scope = scopeFactory.CreateScope();
                var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();
                logger.LogInformation("EventDispatcherWorker started.");
                try
                {
                    logger.LogInformation("Processing Event {EventType} Created on {CreatedAt:yy-MMM-dd ddd}", evt.EventType, evt.CreatedAt);
                    await eventProcessor.ProcessEventAsync(evt, stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error processing event {EventId}", evt.EventId);
                }
            }
        }
            
    }
    
}