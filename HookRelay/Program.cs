using HookRelay.Dtos;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HookRelay;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<HookRelayDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
        builder.Services.AddScoped<WebhookRepository>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        // 1. Register a webhook
        var webhooks = app.MapGroup("/webhooks");
        webhooks.MapPost("/", async(RegisterWebhookRequest request, WebhookRepository repository) =>
        {
            var webhook = Webhook.Create(request.url, request.eventType, request.secret);
            var result = await repository.AddWebHookAsync(webhook);
            return result.IsSuccess ? Results.Created($"/webhooks/{webhook.WebhookId}", null) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("RegisterWebhook");
        // 2. Find a webhook by Id
        webhooks.MapGet("/{id:guid}", async(Guid id, WebhookRepository repository) =>
        {
            var result = await repository.FindWebHookByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("FindWebhookById");
        // 3. Find all webhooks
        webhooks.MapGet("/", async(WebhookRepository repository) =>
        {
            var result = await repository.ListAllWebHooksAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("ListAllWebhooks");
        app.Run();
    }
}