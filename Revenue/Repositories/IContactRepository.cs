using Revenue.Entities;

namespace Revenue.Repositories;

public interface IContactRepository
{
    public Task CreateContractAsync(Contract contract);
    public Task<Contract> GetContractAsync(int contractId);
    public Task<bool> ContractExistsAsync(int contractId);
    public Task SignContractAsync(int contractId);
    public Task CancelContractAsync(int contractId);
    public Task<bool> ActiveContractOfClientExists(int clientId, int softwareId);
}