using System.Threading.Channels;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Services;

public class EventDispatcherWorker(Channel<Event>channel, IServiceScopeFactory scopeFactory, ILogger<EventDispatcherWorker> logger, IHttpClientFactory httpClientFactory):BackgroundService
{
    private const int _maxRetries = 3;
    private HttpClient _httpClient => httpClientFactory.CreateClient();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await foreach (var evt in channel.Reader.ReadAllAsync(stoppingToken))
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