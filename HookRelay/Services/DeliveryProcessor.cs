using HookRelay.Enums;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using HookRelay.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Services;

public class DeliveryProcessor(HookRelayDbContext dbContext, ILogger<DeliveryProcessor> logger):IDeliveryProcessor
{
    public async Task ProcessDeliveryAsync(Guid deliveryId, CancellationToken ct)
    {
        // Fetch the delivery matching the deliveryId include , event and webhook
        // Update the status to processing and save changes
        var delivery = await dbContext.Deliveries
            .Include(d=> d.Webhook)
            .Include(d=>d.Event).FirstOrDefaultAsync(d=> d.DeliveryId == deliveryId, ct);
        if (delivery is null)
        {
            logger.LogInformation("Delivery of id: {deliveryId} was not found", deliveryId);
        }
        delivery.Status = DeliveryStatus.Processing;
        delivery.AttemptCount++;
        await dbContext.SaveChangesAsync(ct);
    }
    
}