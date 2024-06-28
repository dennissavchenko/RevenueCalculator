using Revenue.Entities;

namespace Revenue.Repositories;

public interface ISubscriptionRepository
{
    public Task<bool> SubscriptionExistsAsync(int subscriptionId);
    public Task<Subscription> GetSubscriptionAsync(int subscriptionId);
}