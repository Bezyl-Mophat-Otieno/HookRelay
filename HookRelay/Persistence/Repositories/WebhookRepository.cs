using HookRelay.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Persistence.Repositories;

public class WebhookRepository(HookRelayDbContext dbContext)
{

    public async Task<Result<bool>> AddWebHookAsync(Webhook newWebhook)
    {
        try
        {
            await dbContext.AddAsync(newWebhook);
            var saved =  await dbContext.SaveChangesAsync();
            return saved <= 0 ? Result<bool>.Failure("New webhook entry failed to get saved please try again") : new Result<bool>(true);
        }
        catch (Exception e)
        {
            return Result<bool>.Failure(e.Message);
        }
    }
    public async Task<Result<IEnumerable<Webhook>>> ListAllWebHooksAsync()
    {
        try
        {
            var webhooks = await dbContext.Webhooks.AsNoTracking().ToListAsync();
            return Result<IEnumerable<Webhook>>.Success(webhooks);
        }
        catch (Exception e)
        {
            return Result<IEnumerable<Webhook>>.Failure(e.Message);
        }
    }
    public async Task<Result<Webhook>> FindWebHookByIdAsync(Guid weebhookId)
    {
        try
        {
            var webhook = await dbContext.Webhooks.FindAsync(weebhookId);
            return Result<Webhook>.Success(webhook);
        }
        catch (Exception e)
        {
            return Result<Webhook>.Failure(e.Message);
        }
    }
}