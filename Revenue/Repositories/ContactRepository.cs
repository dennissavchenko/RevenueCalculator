using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;

namespace Revenue.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly SystemContext _systemContext;

    public ContactRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }

    public async Task AddContractAsync(Contract contract)
    {
        await _systemContext.Contracts.AddAsync(contract);
        await _systemContext.SaveChangesAsync();
    }

    public async Task<Contract> GetContractAsync(int contractId)
    {
        return await _systemContext.Contracts.Where(x => x.IdContract == contractId).SingleAsync();
    }

    public async Task<bool> ContractExistsAsync(int contractId)
    {
        return await _systemContext.Contracts.AnyAsync(x => x.IdContract == contractId && !x.IsCancelled);
    }

    public async Task SignContractAsync(int contractId)
    {
        var contract = await GetContractAsync(contractId);
        contract.IsSigned = true;
        await _systemContext.SaveChangesAsync();
    }

    public async Task CancelContractAsync(int contractId)
    {
        var contract = await GetContractAsync(contractId);
        contract.IsCancelled = true;
        await _systemContext.SaveChangesAsync();
    }

    // I assume that active is the one which is not cancelled
    public async Task<bool> ActiveContractOfClientExistsAsync(int clientId, int softwareId)
    {
        return await _systemContext.Contracts.AnyAsync(x => x.IdClient == clientId && x.IdSoftware == softwareId && !x.IsCancelled);
    }

    public async Task<double> GetContactsCurrentRevenueAsync()
    {
        return await _systemContext.Contracts.Where(x => x.IsSigned && !x.IsCancelled).SumAsync(x => x.Price);
    }

    public async Task<double> GetContactsCurrentRevenueForProductAsync(int softwareId)
    {
        return await _systemContext.Contracts.Where(x => x.IsSigned && x.IdSoftware == softwareId && !x.IsCancelled).SumAsync(x => x.Price);
    }

    public async Task<double> GetContactsPredictedRevenueAsync()
    {
        return await _systemContext.Contracts.Where(x => !x.IsCancelled).SumAsync(x => x.Price);
    }

    public async Task<double> GetContactsPredictedRevenueForProductAsync(int softwareId)
    {
        return await _systemContext.Contracts.Where(x => x.IdSoftware == softwareId && !x.IsCancelled).SumAsync(x => x.Price);
    }
}