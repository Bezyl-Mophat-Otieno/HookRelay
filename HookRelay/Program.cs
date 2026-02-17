using HookRelay.Persistence;
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
        webhooks.MapPost("/", (HttpContext httpContext) =>
        {
        }).WithDisplayName("RegisterWebhook");
        app.Run();
    }
}