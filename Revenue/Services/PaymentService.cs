using Revenue.Context;
using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Enums;
using Revenue.Exceptions;
using Revenue.Repositories;

namespace Revenue.Services;

public class PaymentService : IPaymentService
{
    
    private readonly IContactRepository _contactRepository;
    private readonly IContractPaymentRepository _contractPaymentRepository;
    private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionClientRepository _subscriptionClientRepository;
    //private readonly SystemContext _systemContext;

    public PaymentService(IContactRepository contactRepository, IContractPaymentRepository contractPaymentRepository, IClientRepository clientRepository, ISubscriptionRepository subscriptionRepository, ISubscriptionClientRepository subscriptionClientRepository, ISubscriptionPaymentRepository subscriptionPaymentRepository/*, SystemContext systemContext*/)
    {
        _contactRepository = contactRepository;
        _contractPaymentRepository = contractPaymentRepository;
        _clientRepository = clientRepository;
        _subscriptionRepository = subscriptionRepository;
        _subscriptionClientRepository = subscriptionClientRepository;
        _subscriptionPaymentRepository = subscriptionPaymentRepository;
        //_systemContext = systemContext;
    }
    
    public async Task IssueContractPaymentAsync(ContractPaymentDto contractPayment)
    {
        if (!await _contactRepository.ContractExistsAsync(contractPayment.IdContract))
        {
            throw new BadRequestException("Contract with such an ID does not exist or was cancelled!");
        }

        var contract = await _contactRepository.GetContractAsync(contractPayment.IdContract);

        if (contract.IsSigned)
        {
            throw new BadRequestException("Contract with such an ID is already payed and signed!");
        }

        double sum = await _contractPaymentRepository.GetContractPaymentsSumAsync(contractPayment.IdContract);

        if (contractPayment.Date > contract.EndDate.Date)
        {
            await _contactRepository.CancelContractAsync(contractPayment.IdContract);
            await _contractPaymentRepository.ReturnContractPaymentsAsync(contractPayment.IdContract);
            throw new BadRequestException("Contract was outdated. Now it is going to be cancelled. All your previous payments will be returned!");
        }

        if (contractPayment.Amount + sum > contract.Price)
        {
            throw new BadRequestException("Amount issued is too big. You only need to pay " + (contract.Price - sum) + " more!");
        }

        var newPayment = new ContractPayment
        {
            Amount = contractPayment.Amount,
            IdContract = contractPayment.IdContract,
            Date = contractPayment.Date,
            IdPaymentStatus = (int) PaymentStatuses.Accepted
        };

        if ((contractPayment.Amount + sum).Equals(contract.Price))
        {
            await _contractPaymentRepository.AddContractPaymentAsync(newPayment);
            await _contactRepository.SignContractAsync(contractPayment.IdContract);
        }

        if (contractPayment.Amount + sum < contract.Price)
        {
            await _contractPaymentRepository.AddContractPaymentAsync(newPayment);
        }

    }

    public async Task IssueSubscriptionPaymentAsync(SubscriptionPaymentDto payment)
    {
        if (!await _clientRepository.ClientExistsAsync(payment.IdClient))
        {
            throw new BadRequestException("Client with such an ID was not found!");
        }
        
        if (!await _subscriptionRepository.SubscriptionExistsAsync(payment.IdSubscription))
        {
            throw new BadRequestException("Subscription with such an ID was not found!");
        }

        if (!await _subscriptionClientRepository.ClientAlreadySubscribedAsync(payment.IdClient, payment.IdSubscription))
        {
            throw new BadRequestException("Client is not subscribed to the subscription!");
        }

        var subscriptionClient =
            await _subscriptionClientRepository.GetSubscriptionClientAsync(payment.IdClient, payment.IdSubscription);

        if (!payment.Amount.Equals(subscriptionClient.Price))
        {
            throw new BadRequestException("Amount sent is not equal to needed amount!");
        }

        if (subscriptionClient.NextPaymentDate < payment.Date)
        {
            await _subscriptionClientRepository.CancelClientsSubscriptionAsync(payment.IdClient,
                payment.IdSubscription);
            throw new BadRequestException("Payment is late. We have to cancel the client's subscriptions!");
        }
        
        if (subscriptionClient.NextPaymentDate > payment.Date)
        {
            throw new BadRequestException("You have already payed for your subscription this period!");
        }

        var subscriptionPayment = new SubscriptionPayment
        {
            Amount = payment.Amount,
            Date = payment.Date,
            IdSubscriptionClient = subscriptionClient.IdSubscriptionClient
        };

        // using (var transaction = await _systemContext.Database.BeginTransactionAsync())
        // {
        //     try
        //     {
        //         
        //
        //         await transaction.CommitAsync();
        //     }
        //     catch
        //     {
        //         await transaction.RollbackAsync();
        //         throw;
        //     }
        //     
        // }
        
        await _subscriptionPaymentRepository.AddSubscriptionPaymentAsync(subscriptionPayment);
        await _subscriptionClientRepository.UpgradeNextRenewalDateAsync(payment.IdClient, payment.IdSubscription);

    }
}