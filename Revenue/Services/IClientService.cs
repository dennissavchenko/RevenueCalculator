using Revenue.DTOs;

namespace Revenue.Services;

public interface IClientService
{
    public Task AddIndividualClientAsync(IndividualClientDto individualClient);
    public Task AddCompanyClientAsync(CompanyClientDto companyClient);
    public Task UpdateIndividualClientAsync(int clientId, UpdateIndividualClientDto individualClient);
    public Task UpdateCompanyClientAsync(int clientId, UpdateCompanyClientDto companyClient);
    public Task SoftDeleteIndividualClientAsync(int clientId);
}