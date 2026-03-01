using HookRelay.Persistence.Models;

namespace HookRelay.Persistence.Repositories;

public class DeliveryRepository(HookRelayDbContext dbContext)
{    public async Task<bool> AddDeliveryAsync(Delivery newDelivery)
    {
        await dbContext.AddAsync(newDelivery);
        return await dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> AddDeliveryAsync(ICollection<Delivery> newDeliveries)
    {
        await dbContext.AddRangeAsync(newDeliveries);
        return await dbContext.SaveChangesAsync() > 0;
    }
    
    
}