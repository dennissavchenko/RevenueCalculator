using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Repositories;
using Test2.Exceptions;

namespace Revenue.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientService)
    {
        _clientRepository = clientService;
    }

    public async Task AddIndividualClientAsync(IndividualClientDto individualClient)
    {

        if (individualClient.PESEL.Length != 11)
        {
            throw new BadRequestException("PESEL has to consist of 11 digits!");
        }

        if (await _clientRepository.PeselExistsAsync(individualClient.PESEL))
        {
            throw new BadRequestException("Provided PESEL already exists!");
        }
        
        var client = new IndividualClient
        {
            PESEL = individualClient.PESEL,
            FirstName = individualClient.FirstName,
            LastName = individualClient.LastName,
            Address = individualClient.Address,
            Email = individualClient.Email,
            PhoneNumber = individualClient.PhoneNumber
        };
        
        await _clientRepository.AddClientAsync(client);
    }

    public async Task AddCompanyClientAsync(CompanyClientDto companyClient)
    {
       
        if (companyClient.KRS.Length != 14 && companyClient.KRS.Length != 9)
        {
            throw new BadRequestException("KRS is 9 or 14 digits number!");
        }
        
        if (await _clientRepository.KrsExistsAsync(companyClient.KRS))
        {
            throw new BadRequestException("Provided KRS already exists!");
        }
        
        var client = new CompanyClient
        {
            KRS = companyClient.KRS,
            CompanyName = companyClient.CompanyName,
            Address = companyClient.Address,
            Email = companyClient.Email,
            PhoneNumber = companyClient.PhoneNumber
        };
        
        await _clientRepository.AddClientAsync(client);
    }

    public async Task UpdateIndividualClientAsync(int clientId, UpdateIndividualClientDto individualClient)
    {
        if (!await _clientRepository.IndividualClientExistsAsync(clientId))
        {
            throw new NotFoundException("Individual client with such an ID was not found!");
        }

        await _clientRepository.UpdateIndividualClientAsync(clientId, individualClient);

    }

    public async Task UpdateCompanyClientAsync(int clientId, UpdateCompanyClientDto companyClient)
    {
        if (!await _clientRepository.CompanyClientExistsAsync(clientId))
        {
            throw new NotFoundException("Company client with such an ID was not found!");
        }

        await _clientRepository.UpdateCompanyClientAsync(clientId, companyClient);
    }

    public async Task SoftDeleteIndividualClientAsync(int clientId)
    {
        if (!await _clientRepository.IndividualClientExistsAsync(clientId))
        {
            throw new NotFoundException("Individual client with such an ID was not found!");
        }

        await _clientRepository.SoftDeleteIndividualClientAsync(clientId);
        
    }
}