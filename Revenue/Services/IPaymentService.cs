using Revenue.DTOs;

namespace Revenue.Services;

public interface IPaymentService
{
    public Task IssueContractPaymentAsync(ContractPaymentDto contractPayment);
    public Task IssueSubscriptionPaymentAsync(SubscriptionPaymentDto subscriptionPayment);
    
}