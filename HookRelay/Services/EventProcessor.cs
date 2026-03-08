using HookRelay.Enums;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class EventProcessor(WebhookRepository webhookRepository, DeliveryRepository deliveryRepository,  IDeliveryQueue deliveryQueue, ILogger<EventProcessor> logger):IEventProcessor
{
    public async Task ProcessEventAsync(Event evt, CancellationToken ct)
    {
        // Get the webhooks listening to this event type
        // create and persist deliveries to be made
        // enqueue the deliveries for handling later
        var webhooks = await webhookRepository.GetAllWebhooksByEventType(evt.EventType);
        var deliveries = webhooks.Select(whk => Delivery.Create(
            eventId: evt.EventId,
            webhookId: whk.WebhookId,
            status: DeliveryStatus.Pending
        )).ToList();
        var deliveriesSaved = await deliveryRepository.AddDeliveryAsync(deliveries);
        if (!deliveriesSaved) return;
        foreach (var delivery in deliveries)
        {
            logger.LogInformation("Enqueueing delivery: {deliveryId}", delivery.DeliveryId);
            await deliveryQueue.EnqueueAsync(delivery.DeliveryId, ct);
        }
    }
}