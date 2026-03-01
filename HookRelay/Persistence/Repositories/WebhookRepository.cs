using HookRelay.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Persistence.Repositories;

public class WebhookRepository(HookRelayDbContext dbContext)
{

    public async Task<bool> AddWebHookAsync(Webhook newWebhook, CancellationToken ct)
    {
            await dbContext.AddAsync(newWebhook);
            return await dbContext.SaveChangesAsync(ct) > 0;
    }
    public async Task<Result<IEnumerable<Webhook>>> ListAllWebHooksAsync()
    {
            var webhooks = await dbContext.Webhooks.AsNoTracking().ToListAsync();
            return Result<IEnumerable<Webhook>>.Success(webhooks);

    }
    public async Task<Webhook?> FindWebHookByIdAsync(Guid weebhookId, CancellationToken ct)
    {
            return await dbContext.Webhooks.FindAsync(weebhookId, ct);
    }
    public async Task<List<Webhook>>GetAllWebhooksByEventType(string eventType)
    {
            return await dbContext.Webhooks.Where(whk=> whk.EventType == eventType && whk.IsActive == true).ToListAsync();
    }
    public async Task<List<Webhook>> GetAllWebhooks()
    {
            return await dbContext.Webhooks.ToListAsync();
    }
}