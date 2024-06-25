using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.DTOs;
using Revenue.Entities;
using Test2.Exceptions;

namespace Revenue.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly SystemContext _systemContext;

    public ClientRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }

    public async Task AddClientAsync(Client client)
    {
        await _systemContext.Clients.AddAsync(client);
        await _systemContext.SaveChangesAsync();
    }

    public async Task<bool> PeselExistsAsync(string pesel)
    {
        return await _systemContext.IndividualClients.AnyAsync(x => x.PESEL.Equals(pesel));
    }

    public async Task<bool> KrsExistsAsync(string krs)
    {
        return await _systemContext.CompanyClients.AnyAsync(x => x.KRS.Equals(krs));
    }

    public async Task SoftDeleteIndividualClientAsync(int clientId)
    {
        var client = await _systemContext.IndividualClients.Where(x => x.IdClient == clientId).SingleAsync();
        client.IsDeleted = true;
        await _systemContext.SaveChangesAsync();
    }

    public async Task<bool> IndividualClientExistsAsync(int clientId)
    {
        return await _systemContext.IndividualClients.AnyAsync(x => x.IdClient == clientId);
    }
    
    public async Task<bool> CompanyClientExistsAsync(int clientId)
    {
        return await _systemContext.CompanyClients.AnyAsync(x => x.IdClient == clientId);
    }

    public async Task UpdateIndividualClientAsync(int clientId, UpdateIndividualClientDto individualClient)
    {
        var client = await _systemContext.IndividualClients.Where(x => x.IdClient == clientId).SingleAsync();
        
        client.FirstName = individualClient.FirstName;
        client.LastName = individualClient.LastName;
        client.PhoneNumber = individualClient.PhoneNumber;
        client.Address = individualClient.Address;
        client.Email = individualClient.Email;

        await _systemContext.SaveChangesAsync();
    }

    public async Task UpdateCompanyClientAsync(int clientId, UpdateCompanyClientDto companyClient)
    {
        var client = await _systemContext.CompanyClients.Where(x => x.IdClient == clientId).SingleAsync();

        client.CompanyName = companyClient.CompanyName;
        client.PhoneNumber = companyClient.PhoneNumber;
        client.Address = companyClient.Address;
        client.Email = companyClient.Email;

        await _systemContext.SaveChangesAsync();
    }
}