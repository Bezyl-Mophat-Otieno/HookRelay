using System.Security.Cryptography;
using System.Text;
using HookRelay.Enums;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using HookRelay.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Services;

public class DeliveryProcessor(HookRelayDbContext dbContext, ILogger<DeliveryProcessor> logger, IHttpClientFactory clientFactory):IDeliveryProcessor
{
    public async Task ProcessDeliveryAsync(Guid deliveryId, CancellationToken ct)
    {
        var client = clientFactory.CreateClient();
        // Fetch the delivery matching the deliveryId include , event and webhook
        // Update the status to processing and save changes
        var delivery = await dbContext.Deliveries
            .Include(d=> d.Webhook)
            .Include(d=>d.Event).FirstOrDefaultAsync(d=> d.DeliveryId == deliveryId, ct);
        if (delivery is null)
        {
            logger.LogInformation("Delivery of id: {deliveryId} was not found", deliveryId);
            return;
        }
        delivery.Status = DeliveryStatus.Processing;
        delivery.AttemptCount++;
        await dbContext.SaveChangesAsync(ct);

        // create the base request.
        var request = new HttpRequestMessage(
            HttpMethod.Post, 
            delivery.Webhook.Url
            );
        // add payload.
        request.Content = new StringContent(
            delivery.Event.Payload,
            Encoding.UTF8,
            "application/json"
            );
        // add request headers
        request.Headers.Add("X-Webhook-Event", delivery.Event.EventType);
        request.Headers.Add("X-Webhook-Id", delivery.DeliveryId.ToString());
        request.Headers.Add("X-Webhook-Signature", CreateHmacSignature(delivery.Event.Payload, delivery.Webhook.Secret));
        // send the request
        var response = await client.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            await MarkDeliveryFailed(delivery.DeliveryId);
        }

        await MarkDeliverySuccess(delivery.DeliveryId);

    }

    private static string CreateHmacSignature(string payload, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetBytes(payload);
        using var hmac = new HMACSHA3_256(key);
        var hash = hmac.ComputeHash(data);
        return Convert.ToHexString(hash);
    }
    
    private async Task MarkDeliveryFailed(Guid deliveryId)
    {
        await dbContext.Deliveries
            .Where(d => d.DeliveryId == deliveryId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(
                d => d.Status, DeliveryStatus.Failed
            ));

    }    
    private async Task MarkDeliverySuccess(Guid deliveryId)
    {
        await dbContext.Deliveries
            .Where(d => d.DeliveryId == deliveryId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(
                d => d.Status, DeliveryStatus.Succeeded
            ));
    }    
}