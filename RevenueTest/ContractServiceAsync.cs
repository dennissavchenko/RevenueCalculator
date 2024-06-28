using Revenue.Services;

namespace RevenueTest;

using Moq;
using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Exceptions;
using Revenue.Repositories;
using Xunit;

public class ContractServiceTests
{
    private readonly Mock<IContactRepository> _contactRepositoryMock;
    private readonly Mock<ISoftwareRepository> _softwareRepositoryMock;
    private readonly Mock<IDiscountRepository> _discountRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly ContractService _contractService;

    public ContractServiceTests()
    {
        _contactRepositoryMock = new Mock<IContactRepository>();
        _softwareRepositoryMock = new Mock<ISoftwareRepository>();
        _discountRepositoryMock = new Mock<IDiscountRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();

        _contractService = new ContractService(
            _contactRepositoryMock.Object,
            _softwareRepositoryMock.Object,
            _discountRepositoryMock.Object,
            _clientRepositoryMock.Object
        );
    }

    [Fact]
    public async Task CreateContractAsync_ValidContract_CreatesContractSuccessfully()
    {
        var contractDto = new ContractDto
        {
            IdClient = 1,
            IdSoftware = 1,
            EndDate = DateTime.Today.AddDays(30),
            AdditionalYearsOfSupport = 1
        };

        var software = new Software
        {
            IdSoftware = contractDto.IdSoftware,
            UpfrontCost = 1000,
            CurrentVersion = "1.0"
        };

        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(contractDto.IdSoftware)).ReturnsAsync(true);
        _softwareRepositoryMock.Setup(repo => repo.GetSoftwareAsync(contractDto.IdSoftware)).ReturnsAsync(software);
        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(contractDto.IdClient)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.HasContractOrSubscriptionAsync(contractDto.IdClient)).ReturnsAsync(false);
        _contactRepositoryMock.Setup(repo => repo.ActiveContractOfClientExistsAsync(contractDto.IdClient, contractDto.IdSoftware)).ReturnsAsync(false);
        _discountRepositoryMock.Setup(repo => repo.GetMaxDiscountAsync(contractDto.IdSoftware, It.IsAny<DateTime>())).ReturnsAsync(10);
        
        await _contractService.CreateContractAsync(contractDto);
        
        _contactRepositoryMock.Verify(repo => repo.AddContractAsync(It.IsAny<Contract>()), Times.Once);
    }

    [Fact]
    public async Task CreateContractAsync_InvalidTimeRange_ThrowsBadRequestException()
    {
        var contractDto = new ContractDto
        {
            IdClient = 1,
            IdSoftware = 1,
            EndDate = DateTime.Today.AddDays(1),
            AdditionalYearsOfSupport = 1
        };
        
        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(contractDto.IdSoftware)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(contractDto.IdClient)).ReturnsAsync(true);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _contractService.CreateContractAsync(contractDto));
        Assert.Equal("The time range should be at least 3 days and at most 30 days.", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_SoftwareNotFound_ThrowsBadRequestException()
    {
        var contractDto = new ContractDto
        {
            IdClient = 1,
            IdSoftware = 10,
            EndDate = DateTime.Today.AddDays(30),
            AdditionalYearsOfSupport = 1
        };

        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(contractDto.IdSoftware)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _contractService.CreateContractAsync(contractDto));
        Assert.Equal("Software with such an ID does not exist!", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ClientNotFound_ThrowsBadRequestException()
    {
        var contractDto = new ContractDto
        {
            IdClient = 10,
            IdSoftware = 1,
            EndDate = DateTime.Today.AddDays(30),
            AdditionalYearsOfSupport = 1
        };

        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(contractDto.IdSoftware)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(contractDto.IdClient)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _contractService.CreateContractAsync(contractDto));
        Assert.Equal("Client with such an ID does not exist!", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ActiveContractExistsForClient_ThrowsBadRequestException()
    {
        var contractDto = new ContractDto
        {
            IdClient = 1,
            IdSoftware = 1,
            EndDate = DateTime.Today.AddDays(30),
            AdditionalYearsOfSupport = 1
        };

        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(contractDto.IdSoftware)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(contractDto.IdClient)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.HasContractOrSubscriptionAsync(contractDto.IdClient)).ReturnsAsync(true);
        _contactRepositoryMock.Setup(repo => repo.ActiveContractOfClientExistsAsync(contractDto.IdClient, contractDto.IdSoftware)).ReturnsAsync(true);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _contractService.CreateContractAsync(contractDto));
        Assert.Equal("Client already has an active contract for the software!", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_InvalidAdditionalYearsOfSupport_ThrowsBadRequestException()
    {
        var contractDto = new ContractDto
        {
            IdClient = 1,
            IdSoftware = 1,
            EndDate = DateTime.Today.AddDays(30),
            AdditionalYearsOfSupport = -1
        };

        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(contractDto.IdSoftware)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(contractDto.IdClient)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.HasContractOrSubscriptionAsync(contractDto.IdClient)).ReturnsAsync(false);
        _contactRepositoryMock.Setup(repo => repo.ActiveContractOfClientExistsAsync(contractDto.IdClient, contractDto.IdSoftware)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _contractService.CreateContractAsync(contractDto));
        Assert.Equal("Maximum number of additional years of support is 3", exception.Message);
    }

}
