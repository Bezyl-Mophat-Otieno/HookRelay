using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using HookRelay.Services.Abstractions;

namespace HookRelay.Services;

public class WebhookService(WebhookRepository webhookRepository):IWebhookService
{
    public async Task<Result<Webhook>> CreateWebhookAsync(Webhook webbhook, CancellationToken ct = default)
    {
        try
        {
            var newWebhook = await webhookRepository.AddWebHookAsync(webbhook, ct);
            return newWebhook ? Result<Webhook>.Success(webbhook) : Result<Webhook>.Failure("Webhook not registered");
        }
        catch (Exception e)
        {
            return Result<Webhook>.Failure(e.Message);
        }
    }

    public async Task<Result<Webhook>> GetWebhookByIdAsync(Guid webhookId, CancellationToken ct = default)
    {
        try
        {
            var webhook = await webhookRepository.FindWebHookByIdAsync(webhookId, ct);
            return webhook is not null
                ? Result<Webhook>.Success(webhook)
                : Result<Webhook>.Failure("Webhook not found");
        }
        catch (Exception e)
        {
            return Result<Webhook>.Failure(e.Message);
        }    
    }

    public async Task<Result<List<Webhook>>> ListAllWebhookAsync()
    {
        try
        {
            var webhooks = await webhookRepository.GetAllWebhooks();
            return Result<List<Webhook>>.Success(webhooks);
        }
        catch (Exception e)
        {
            return Result<List<Webhook>>.Failure(e.Message);
        }       
    }
}