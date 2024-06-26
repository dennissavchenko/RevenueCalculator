using Revenue.Entities;

namespace Revenue.Repositories;

public interface IPaymentRepository
{
    public Task IssuePaymentAsync(ContractPayment payment);
    public Task ReturnPaymentsAsync(int contractId);
    public Task<double> GetPaymentsSumAsync(int contractId);
}