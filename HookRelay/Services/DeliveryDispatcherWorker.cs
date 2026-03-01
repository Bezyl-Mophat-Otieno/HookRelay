using System.Threading.Channels;
using HookRelay.Persistence.Models;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class DeliveryDispatcherWorker(Channel<Delivery>channel, ILogger<DeliveryDispatcherWorker> logger, IServiceScopeFactory scopeFactory):BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await foreach (var delivery in channel.Reader.ReadAllAsync(stoppingToken))
            {
                var scope = scopeFactory.CreateScope();
                var deliveryProcessor = scope.ServiceProvider.GetRequiredService<IDeliveryProcessor>();
                logger.LogInformation("DeliveryDispatcherWorker started.");
                try
                {
                    logger.LogInformation("Processing Delivery {deliveryId} Created on {CreatedAt:yy-MMM-dd ddd}", delivery.DeliveryId, delivery.CreatedAt);
                    await deliveryProcessor.ProcessDeliveryAsync(delivery.DeliveryId, stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error processing delivery {DeliveryId}", delivery.DeliveryId);
                }
            }
        }
            
    }
    
}