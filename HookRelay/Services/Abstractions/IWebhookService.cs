using HookRelay.Persistence;
using HookRelay.Persistence.Models;

namespace HookRelay.Services.Abstractions;

public interface IWebhookService
{
    public Task<Result<Webhook>> CreateWebhookAsync(Webhook webbhook, CancellationToken ct=default);
    public Task<Result<Webhook>> GetWebhookByIdAsync(Guid webhookId, CancellationToken ct=default);
    public Task<Result<List<Webhook>>>ListAllWebhookAsync();
}