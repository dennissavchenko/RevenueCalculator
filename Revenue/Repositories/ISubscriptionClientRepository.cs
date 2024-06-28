using Revenue.Entities;

namespace Revenue.Repositories;

public interface ISubscriptionClientRepository
{
    public Task<int> AddSubscriptionClientAsync(SubscriptionClient subscriptionClient);
    public Task<bool> ClientAlreadySubscribedAsync(int clientId, int subscriptionId);
    public Task<SubscriptionClient> GetSubscriptionClientAsync(int clientId, int subscriptionId);
    public Task CancelClientsSubscriptionAsync(int clientId, int subscriptionId);
    public Task UpgradeNextRenewalDateAsync(int clientId, int subscriptionId);
    public Task<IEnumerable<SubscriptionClient>> GetAllSubscriptionClientsAsync();
    public Task<IEnumerable<SubscriptionClient>> GetAllSubscriptionClientsForProductAsync(int softwareId);
}