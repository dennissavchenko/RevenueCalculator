using Revenue.Entities;

namespace Revenue.Repositories;

public interface IContactRepository
{
    public Task AddContractAsync(Contract contract);
    public Task<Contract> GetContractAsync(int contractId);
    public Task<bool> ContractExistsAsync(int contractId);
    public Task SignContractAsync(int contractId);
    public Task CancelContractAsync(int contractId);
    public Task<bool> ActiveContractOfClientExistsAsync(int clientId, int softwareId);
    public Task<double> GetContactsCurrentRevenueAsync();
    public Task<double> GetContactsCurrentRevenueForProductAsync(int softwareId);
    public Task<double> GetContactsPredictedRevenueAsync();
    public Task<double> GetContactsPredictedRevenueForProductAsync(int softwareId);
    
}