using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;

namespace Revenue.Repositories;

public class SubscriptionPaymentRepository : ISubscriptionPaymentRepository
{
    private readonly SystemContext _systemContext;

    public SubscriptionPaymentRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }

    public async Task AddSubscriptionPaymentAsync(SubscriptionPayment subscriptionPayment)
    {
        await _systemContext.SubscriptionPayments.AddAsync(subscriptionPayment);
    }

    public async Task<double> GetSubscriptionsCurrentRevenueAsync()
    {
        return await _systemContext.SubscriptionPayments.SumAsync(x => x.Amount);
    }

    public async Task<double> GetSubscriptionsCurrentRevenueForProductAsync(int softwareId)
    {
        return await _systemContext.SubscriptionPayments.Where(x => x.SubscriptionClient.Subscription.IdSoftware == softwareId).SumAsync(x => x.Amount);
    }
    
}