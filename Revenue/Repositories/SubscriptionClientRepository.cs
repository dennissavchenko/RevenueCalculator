using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;

namespace Revenue.Repositories;

public class SubscriptionClientRepository : ISubscriptionClientRepository
{
    private readonly SystemContext _systemContext;

    public SubscriptionClientRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }
    
    public async Task<int> AddSubscriptionClientAsync(SubscriptionClient subscriptionClient)
    {
        await _systemContext.SubscriptionClients.AddAsync(subscriptionClient);
        await _systemContext.SaveChangesAsync();
        return await _systemContext.SubscriptionClients.MaxAsync(x => x.IdSubscriptionClient);
    }

    public async Task<bool> ClientAlreadySubscribedAsync(int clientId, int subscriptionId)
    {
        return await _systemContext.SubscriptionClients.AnyAsync(x =>
            x.IdClient == clientId && x.IdSubscription == subscriptionId && !x.IsCancelled);
    }

    public async Task<SubscriptionClient> GetSubscriptionClientAsync(int clientId, int subscriptionId)
    {
        return await _systemContext.SubscriptionClients
            .Where(x => x.IdSubscription == subscriptionId && x.IdClient == clientId)
            .Include(x => x.Subscription)
            .SingleAsync();
    }

    public async Task CancelClientsSubscriptionAsync(int clientId, int subscriptionId)
    {
        var subscriptionClient = await GetSubscriptionClientAsync(clientId, subscriptionId);
        subscriptionClient.IsCancelled = true;
        await _systemContext.SaveChangesAsync();
    }

    public async Task UpgradeNextRenewalDateAsync(int clientId, int subscriptionId)
    {
        var subscriptionClient = await GetSubscriptionClientAsync(clientId, subscriptionId);
        subscriptionClient.NextPaymentDate =
            subscriptionClient.NextPaymentDate.AddDays(subscriptionClient.Subscription.RenewalPeriodDays);
        await _systemContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<SubscriptionClient>> GetAllSubscriptionClientsAsync()
    {
        return await _systemContext.SubscriptionClients.Include(x => x.Subscription).ToListAsync();
    }

    public async Task<IEnumerable<SubscriptionClient>> GetAllSubscriptionClientsForProductAsync(int softwareId)
    {
        return await _systemContext.SubscriptionClients.Where(x => x.Subscription.IdSoftware == softwareId).Include(x => x.Subscription).ToListAsync();
    }
}