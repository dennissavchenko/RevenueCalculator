using Revenue.Entities;

namespace Revenue.Repositories;

public interface ISubscriptionPaymentRepository
{
    public Task AddSubscriptionPaymentAsync(SubscriptionPayment subscriptionPayment);
    public Task<double> GetSubscriptionsCurrentRevenueAsync();
    public Task<double> GetSubscriptionsCurrentRevenueForProductAsync(int softwareId);
}