using Revenue.Constant;
using Revenue.Context;
using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Exceptions;
using Revenue.Repositories;

namespace Revenue.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IClientRepository _clientRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionClientRepository _subscriptionClientRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
    private readonly SystemContext _systemContext;

    public SubscriptionService(IClientRepository clientRepository, ISubscriptionRepository subscriptionRepository, ISubscriptionClientRepository subscriptionClientRepository, IDiscountRepository discountRepository, ISubscriptionPaymentRepository subscriptionPaymentRepository, SystemContext systemContext)
    {
        _clientRepository = clientRepository;
        _subscriptionRepository = subscriptionRepository;
        _subscriptionClientRepository = subscriptionClientRepository;
        _discountRepository = discountRepository;
        _subscriptionPaymentRepository = subscriptionPaymentRepository;
        _systemContext = systemContext;
    }

    public async Task BuySubscriptionAsync(BuySubscriptionDto buySubscription)
    {
        
        if (!await _clientRepository.ClientExistsAsync(buySubscription.IdClient))
        {
            throw new BadRequestException("Client with such an ID was not found");
        }
        
        if (!await _subscriptionRepository.SubscriptionExistsAsync(buySubscription.IdSubscription))
        {
            throw new BadRequestException("Subscription with such an ID was not found");
        }
        
        if (await _subscriptionClientRepository.ClientAlreadySubscribedAsync(buySubscription.IdClient,
                buySubscription.IdSubscription))
        {
            throw new BadRequestException("This client has already subscribed to the subscription!");
        }

        var subscription = await _subscriptionRepository.GetSubscriptionAsync(buySubscription.IdSubscription);

        double discount = await _discountRepository.GetMaxDiscountAsync(subscription.IdSoftware, DateTime.Today);

        double loyalClientDiscount = 0;

        if (await _clientRepository.HasContractOrSubscriptionAsync(buySubscription.IdClient))
            loyalClientDiscount = SubscriptionConstants.LoyalClientDiscount;

        double firstPayment = subscription.Price - subscription.Price * (discount / 100);

        firstPayment -= firstPayment * loyalClientDiscount;

        var subscriptionClient = new SubscriptionClient
        {
            IdClient = buySubscription.IdClient,
            IdSubscription = buySubscription.IdSubscription,
            Price = subscription.Price - subscription.Price * loyalClientDiscount,
            IsCancelled = false,
            NextPaymentDate = DateTime.Today.AddDays(subscription.RenewalPeriodDays),
        };
        
        using (var transaction = await _systemContext.Database.BeginTransactionAsync())
        {
            try
            {
                int scId = await _subscriptionClientRepository.AddSubscriptionClientAsync(subscriptionClient);
        
                var subscriptionPayment = new SubscriptionPayment
                {
                    Amount = firstPayment,
                    Date = DateTime.Today,
                    IdSubscriptionClient = scId
                };
        
                await _subscriptionPaymentRepository.AddSubscriptionPaymentAsync(subscriptionPayment);
        
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }

    }
    
}