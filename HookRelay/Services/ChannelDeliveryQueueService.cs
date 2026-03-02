using System.Threading.Channels;
using HookRelay.Persistence.Models;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class ChannelDeliveryQueueService(Channel<Guid> channel): IDeliveryQueue
{
    public ValueTask EnqueueAsync(Guid deliveryId, CancellationToken ct = default)
    {
        return channel.Writer.TryWrite(deliveryId) ? ValueTask.CompletedTask : channel.Writer.WriteAsync(deliveryId, ct);
    }
    
}