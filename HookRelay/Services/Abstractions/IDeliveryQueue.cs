using HookRelay.Persistence.Models;

namespace HookRelay.Services.Abstractions;

public interface IDeliveryQueue
{
    public ValueTask EnqueueAsync(Delivery delivery, CancellationToken ct = default);
}