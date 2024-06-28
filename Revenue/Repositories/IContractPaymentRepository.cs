using Revenue.Entities;

namespace Revenue.Repositories;

public interface IContractPaymentRepository
{
    public Task AddContractPaymentAsync(ContractPayment payment);
    public Task ReturnContractPaymentsAsync(int contractId);
    public Task<double> GetContractPaymentsSumAsync(int contractId);
}