using System.Threading.Channels;
using HookRelay.Dtos;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using HookRelay.Services;
using HookRelay.Services.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HookRelay;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var channel = Channel.CreateBounded<Event>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<HookRelayDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
        builder.Services.AddScoped<WebhookRepository>();
        builder.Services.AddScoped<EventRepository>();
        builder.Services.AddScoped<IQueueEventService, ChannelQueueService>();
        builder.Services.AddScoped<IEventService, EventService>();
        builder.Services.AddSingleton(channel);
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
        var webhooks = app.MapGroup("/webhooks");
        webhooks.MapPost("/", async(RegisterWebhookRequest request, WebhookRepository repository) =>
        {
            var webhook = Webhook.Create(request.url, request.eventType, request.secret);
            var result = await repository.AddWebHookAsync(webhook);
            return result.IsSuccess ? Results.Created($"/webhooks/{webhook.WebhookId}", null) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("RegisterWebhook");
        webhooks.MapGet("/{id:guid}", async(Guid id, WebhookRepository repository) =>
        {
            var result = await repository.FindWebHookByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("FindWebhookById");
        webhooks.MapGet("/", async(WebhookRepository repository) =>
        {
            var result = await repository.ListAllWebHooksAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("ListAllWebhooks");

        var events = app.MapGroup("/events");
        events.MapPost("/", async (CreateEventRequest request, IEventService eventService) =>
        {
            var newEvent = Event.Create(request.eventType, request.payload);
            var result = await eventService.CreateEventAsync(newEvent);
            return result.IsSuccess ? Results.Created($"/events/{newEvent.EventId}", null) : Results.BadRequest(result.ErrorMessage);

        }).WithDisplayName("CreateEvent");
        events.MapGet("/{id:guid}", async (Guid id, IEventService eventService) =>
        {
            var result = await eventService.GetEventByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("GetEventById");
        app.Run();
    }
}