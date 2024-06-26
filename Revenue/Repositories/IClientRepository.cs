using Revenue.DTOs;
using Revenue.Entities;

namespace Revenue.Repositories;

public interface IClientRepository
{
    public Task AddClientAsync(Client client);
    public Task<bool> PeselExistsAsync(string pesel);
    public Task<bool> KrsExistsAsync(string krs);
    public Task SoftDeleteIndividualClientAsync(int clientId);
    public Task<bool> IndividualClientExistsAsync(int clientId);
    public Task<bool> CompanyClientExistsAsync(int clientId);
    public Task UpdateIndividualClientAsync(int clientId, UpdateIndividualClientDto individualClient);
    public Task UpdateCompanyClientAsync(int clientId, UpdateCompanyClientDto companyClient);
    public Task<bool> HasContractOrSubscriptionAsync(int clientId);
    public Task<bool> ClientExistsAsync(int clientId);
}