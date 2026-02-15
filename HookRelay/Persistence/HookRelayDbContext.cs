using HookRelay.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HookRelay.Persistence;

public class HookRelayDbContext: DbContext
{
    public DbSet<Webhook> Webhooks =>  Set<Webhook>();
    public DbSet<Event> Events =>  Set<Event>();
    public DbSet<Delivery> Deliveries =>  Set<Delivery>();

    public HookRelayDbContext(DbContextOptions<HookRelayDbContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(HookRelayDbContext).Assembly
        );
    }
}