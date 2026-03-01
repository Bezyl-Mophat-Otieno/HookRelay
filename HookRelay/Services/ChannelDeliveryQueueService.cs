using System.Threading.Channels;
using HookRelay.Persistence.Models;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class ChannelDeliveryQueueService(Channel<Delivery> channel): IDeliveryQueue
{
    public ValueTask EnqueueAsync(Delivery delivery, CancellationToken ct = default)
    {
        return channel.Writer.TryWrite(delivery) ? ValueTask.CompletedTask : channel.Writer.WriteAsync(delivery, ct);
    }
}