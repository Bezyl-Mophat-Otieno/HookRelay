using System.Threading.Channels;
using HookRelay.Persistence.Models;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class ChannelQueueService(Channel<Event> channel): IQueueEventService
{
    public ValueTask EnqueueAsync(Event evt, CancellationToken ct = default)
    {
        return channel.Writer.TryWrite(evt) ? ValueTask.CompletedTask : channel.Writer.WriteAsync(evt, ct);
    }
}