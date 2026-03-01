using HookRelay.Persistence.Models;

namespace HookRelay.Services.Abstractions;

public interface IDeliveryQueue
{
    public ValueTask EnqueueAsync(Guid deliveryId, CancellationToken ct = default);
}