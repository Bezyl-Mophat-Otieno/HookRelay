using System.Threading.Channels;
using HookRelay.Dtos;
using HookRelay.Persistence;
using HookRelay.Persistence.Models;
using HookRelay.Persistence.Repositories;
using HookRelay.Services;
using HookRelay.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HookRelay;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adding an in memory channel that would act  like a in-memory queue for our events .
        var eventsChannel = Channel.CreateBounded<Event>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        var deliveryChannel = Channel.CreateBounded<Delivery>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<HookRelayDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
        builder.Services.AddScoped<WebhookRepository>();
        builder.Services.AddScoped<EventRepository>();
        builder.Services.AddScoped<DeliveryRepository>();
        builder.Services.AddScoped<IEventQueue, ChannelEventQueueService>();
        builder.Services.AddScoped<IDeliveryQueue, ChannelDeliveryQueueService>();
        builder.Services.AddScoped<IEventService, EventService>();
        builder.Services.AddScoped<IWebhookService, WebhookService>();
        builder.Services.AddSingleton(eventsChannel);
        builder.Services.AddSingleton(deliveryChannel);
        builder.Services.AddScoped<IEventProcessor, EventProcessor>();
        builder.Services.AddScoped<IDeliveryProcessor, DeliveryProcessor>();
        builder.Services.AddHostedService<EventDispatcherWorker>();
        builder.Services.AddHostedService<DeliveryDispatcherWorker>();
        builder.Services.AddHttpClient();
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
        webhooks.MapPost("/", async(RegisterWebhookRequest request, IWebhookService webhookService) =>
        {
            var webhook = Webhook.Create(request.url, request.eventType, request.secret);
            var result = await webhookService.CreateWebhookAsync(webhook);
            return result.IsSuccess ? Results.Created($"/webhooks/{webhook.WebhookId}", null) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("RegisterWebhook");
        webhooks.MapGet("/{id:guid}", async (Guid id, IWebhookService webhookService) =>
        {
            var result = await webhookService.GetWebhookByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("FindWebhookById");
        webhooks.MapGet("/", async(IWebhookService webhookService) =>
        {
            var result = await webhookService.ListAllWebhookAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("ListAllWebhooks");

        var events = app.MapGroup("/events");
        events.MapPost("/", async (CreateEventRequest request, IEventService eventService) =>
        {
            var newEvent = Event.Create(request.eventType, request.payload.GetRawText());
            var result = await eventService.CreateEventAsync(newEvent);
            return result.IsSuccess ? Results.Created($"/events/{newEvent.EventId}", null) : Results.BadRequest(result.ErrorMessage);

        }).WithDisplayName("CreateEvent");
        events.MapGet("/{id:guid}", async (Guid id, IEventService eventService) =>
        {
            var result = await eventService.GetEventByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("GetEventById");
        events.MapGet("/", async(IEventService eventService) =>
        {
            var result = await eventService.ListAllEventAsync();
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.ErrorMessage);
        }).WithDisplayName("ListAllWebhooks");
        app.Run();
    }
}