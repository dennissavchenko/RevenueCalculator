using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;

namespace Revenue.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SystemContext _systemContext;

    public SubscriptionRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }
    
    public async Task<bool> SubscriptionExistsAsync(int subscriptionId)
    {
        return await _systemContext.Subscriptions.AnyAsync(x => x.IdSubscription == subscriptionId);
    }

    public async Task<Subscription> GetSubscriptionAsync(int subscriptionId)
    {
        return await _systemContext.Subscriptions.Where(x => x.IdSubscription == subscriptionId).SingleAsync();
    }
    
}