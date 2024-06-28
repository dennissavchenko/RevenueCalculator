using Revenue.Constant;
using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Exceptions;
using Revenue.Repositories;

namespace Revenue.Services;

public class ContractService : IContractService
{
    private readonly IContactRepository _contactRepository;
    private readonly ISoftwareRepository _softwareRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IClientRepository _clientRepository;

    public ContractService(IContactRepository contactRepository, ISoftwareRepository softwareRepository, IDiscountRepository discountRepository, IClientRepository clientRepository)
    {
        _contactRepository = contactRepository;
        _softwareRepository = softwareRepository;
        _discountRepository = discountRepository;
        _clientRepository = clientRepository;
    }

    public async Task CreateContractAsync(ContractDto contract)
    {
        
        DateTime today = DateTime.Today;
        
        TimeSpan range = contract.EndDate - today;

        if (range.TotalDays > ContractConstants.MaxTimeRange || range.TotalDays < ContractConstants.MinTimeRange)
        {
            throw new BadRequestException("The time range should be at least " + ContractConstants.MinTimeRange + " days and at most " + ContractConstants.MaxTimeRange + " days.");
        }

        if (!await _softwareRepository.SoftwareExistsAsync(contract.IdSoftware))
        {
            throw new BadRequestException("Software with such an ID does not exist!");
        }
        
        if (!await _clientRepository.ClientExistsAsync(contract.IdClient))
        {
            throw new BadRequestException("Client with such an ID does not exist!");
        }
        
        if (await _contactRepository.ActiveContractOfClientExistsAsync(contract.IdClient, contract.IdSoftware))
        {
            throw new BadRequestException("Client already has an active contract for the software!");
        }
        
        if (contract.AdditionalYearsOfSupport < 0 ||
            contract.AdditionalYearsOfSupport > ContractConstants.MaxAdditionalYearsOfSupport)
        {
            throw new BadRequestException("Maximum number of additional years of support is " + ContractConstants.MaxAdditionalYearsOfSupport);
        }

        var software = await _softwareRepository.GetSoftwareAsync(contract.IdSoftware);

        double discount = await _discountRepository.GetMaxDiscountAsync(contract.IdSoftware, today);

        double loyalClientDiscount = 0;

        if (await _clientRepository.HasContractOrSubscriptionAsync(contract.IdClient)) loyalClientDiscount = ContractConstants.LoyalClientDiscount;

        double price = software.UpfrontCost - software.UpfrontCost * (discount / 100) + contract.AdditionalYearsOfSupport * ContractConstants.ExtraYearOfSupportPrice;

        price -= price * loyalClientDiscount;
        
        var newContract = new Contract
        {
            StartDate = today,
            EndDate = contract.EndDate,
            Price = price,
            AdditionalYearsOfSupport = contract.AdditionalYearsOfSupport,
            IsSigned = false,
            IsCancelled = false,
            SoftwareVersion = software.CurrentVersion,
            IdSoftware = contract.IdSoftware,
            IdClient = contract.IdClient
        };

        await _contactRepository.AddContractAsync(newContract);

    }
}