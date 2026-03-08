using System.Threading.Channels;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using HookRelay.Services;
using HookRelay.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Extensions;

public static partial class DependencyInjection
{
    public static IServiceCollection AddPresentationLayerConfig(this IServiceCollection services, IConfigurationManager configuration)
    {
        // Adding an in memory channel that would act like a in-memory queue for our events .
        var eventsChannel = Channel.CreateBounded<Event>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        var deliveriesChannel = Channel.CreateBounded<Guid>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        services.AddAuthorization();
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddDbContext<HookRelayDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
        services.AddScoped<WebhookRepository>();
        services.AddScoped<EventRepository>();
        services.AddScoped<DeliveryRepository>();
        services.AddScoped<IEventQueue, ChannelEventQueueService>();
        services.AddScoped<IDeliveryQueue, ChannelDeliveryQueueService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IWebhookService, WebhookService>();
        services.AddSingleton(eventsChannel);
        services.AddSingleton(deliveriesChannel);
        services.AddScoped<IEventProcessor, EventProcessor>();
        services.AddScoped<IDeliveryProcessor, DeliveryProcessor>();
        services.AddHostedService<EventDispatcherWorker>();
        services.AddHostedService<DeliveryDispatcherWorker>();
        services.AddHttpClient();
        return services;
    }
}