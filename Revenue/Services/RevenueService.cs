using Revenue.Entities;
using Revenue.Exceptions;
using Revenue.Repositories;

namespace Revenue.Services;

public class RevenueService : IRevenueService
{
    private readonly IContactRepository _contactRepository;
    private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
    private readonly ISoftwareRepository _softwareRepository;
    private readonly ISubscriptionClientRepository _subscriptionClientRepository;
    private readonly ExchangeRateService _exchangeRateService;

    public RevenueService(IContactRepository contactRepository, ISubscriptionPaymentRepository subscriptionPaymentRepository, ISoftwareRepository softwareRepository, ISubscriptionClientRepository subscriptionClientRepository, ExchangeRateService exchangeRateService)
    {
        _contactRepository = contactRepository;
        _subscriptionPaymentRepository = subscriptionPaymentRepository;
        _softwareRepository = softwareRepository;
        _subscriptionClientRepository = subscriptionClientRepository;
        _exchangeRateService = exchangeRateService;
    }

    public async Task<double> GetCurrentRevenueAsync()
    {
        return await _contactRepository.GetContactsCurrentRevenueAsync() +
               await _subscriptionPaymentRepository.GetSubscriptionsCurrentRevenueAsync();
    }

    public async Task<double> GetCurrentRevenueForProductAsync(int softwareId)
    {
        if (!await _softwareRepository.SoftwareExistsAsync(softwareId))
        {
            throw new NotFoundException("Software with such an ID does not exist!");
        }

        return await _contactRepository.GetContactsCurrentRevenueForProductAsync(softwareId) +
               await _subscriptionPaymentRepository.GetSubscriptionsCurrentRevenueForProductAsync(softwareId);

    }

    public async Task<double> GetCurrentRevenueInCurrencyAsync(string currencyCode)
    {
        double revenue = await GetCurrentRevenueAsync();
        double exchangeRate = await _exchangeRateService.GetExchangeRateAsync(currencyCode);
        revenue /= exchangeRate;
        return revenue;
    }

    public async Task<double> GetCurrentRevenueForProductInCurrencyAsync(int softwareId, string currencyCode)
    {
        
        if (!await _softwareRepository.SoftwareExistsAsync(softwareId))
        {
            throw new NotFoundException("Software with such an ID does not exist!");
        }
        
        double revenue = await GetCurrentRevenueForProductAsync(softwareId);
        double exchangeRate = await _exchangeRateService.GetExchangeRateAsync(currencyCode);
        revenue /= exchangeRate;
        return revenue;
    }

    private double GetSubscriptionsRevenue(IEnumerable<SubscriptionClient> subscriptionsClients,
        int predictionPeriodDays)
    {
        double revenue = 0;
        foreach (var sc in subscriptionsClients)
        {
            if (DateTime.Today.AddDays(predictionPeriodDays) >= sc.NextPaymentDate)
            {
                TimeSpan skippedGap = sc.NextPaymentDate - DateTime.Today;
                int futurePayments = (predictionPeriodDays - skippedGap.Days) / sc.Subscription.RenewalPeriodDays;
                revenue += sc.Price * (futurePayments + 1);
            }
        }

        return revenue;
    }

    public async Task<double> GetPredictedRevenueAsync(int predictionPeriodDays)
    {

        double revenue = 0;

        revenue += await _subscriptionPaymentRepository.GetSubscriptionsCurrentRevenueAsync();
        
        var subscriptionsClients = await _subscriptionClientRepository.GetAllSubscriptionClientsAsync();

        revenue += GetSubscriptionsRevenue(subscriptionsClients, predictionPeriodDays);

        revenue += await _contactRepository.GetContactsPredictedRevenueAsync();

        return revenue;

    }

    public async Task<double> GetPredictedRevenueForProductAsync(int softwareId, int predictionPeriodDays)
    {
        
        if (!await _softwareRepository.SoftwareExistsAsync(softwareId))
        {
            throw new NotFoundException("Software with such an ID does not exist!");
        }
        
        double revenue = 0;

        revenue += await _subscriptionPaymentRepository.GetSubscriptionsCurrentRevenueForProductAsync(softwareId);

        var subscriptionsClients =
            await _subscriptionClientRepository.GetAllSubscriptionClientsForProductAsync(softwareId);
        
        revenue += GetSubscriptionsRevenue(subscriptionsClients, predictionPeriodDays);
        
        revenue += await _contactRepository.GetContactsPredictedRevenueForProductAsync(softwareId);

        return revenue;
        
    }
}