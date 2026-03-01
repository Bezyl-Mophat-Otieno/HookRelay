using HookRelay.Persistence.Models;

namespace HookRelay.Services.Abstractions;

public interface IDeliveryProcessor
{
   public Task ProcessDeliveryAsync(Guid deliveryId, CancellationToken ct);
}