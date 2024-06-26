using Revenue.DTOs;

namespace Revenue.Services;

public interface IContractService
{
    public Task CreateContractAsync(ContractDto contract);
}