using Revenue.DTOs;

namespace Revenue.Services;

public interface ISubscriptionService
{
    public Task BuySubscriptionAsync(BuySubscriptionDto buySubscription);
}