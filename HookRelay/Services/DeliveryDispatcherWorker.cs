using System.Threading.Channels;
using HookRelay.Persistence.Models;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class DeliveryDispatcherWorker(Channel<Guid>deliveriesChannel, IServiceScopeFactory scopeFactory, ILogger<DeliveryDispatcherWorker> logger, IHttpClientFactory httpClientFactory):BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("DeliveryDispatcherWorker running...");
            await foreach (var deliveryId in deliveriesChannel.Reader.ReadAllAsync(stoppingToken))
            {
                var scope = scopeFactory.CreateScope();
                var deliveryProcessor = scope.ServiceProvider.GetRequiredService<IDeliveryProcessor>();
                logger.LogInformation("DeliveryDispatcherWorker started.");
                try
                {
                    logger.LogInformation("Processing Delivery {deliveryId}.", deliveryId);
                    await deliveryProcessor.ProcessDeliveryAsync(deliveryId, stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error processing delivery {DeliveryId}", deliveryId);
                }
            }
        }
            
    }
    
}