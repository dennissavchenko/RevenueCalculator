using Moq;
using Revenue.Constant;
using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Exceptions;
using Revenue.Repositories;
using Revenue.Services;

namespace RevenueTest;

public class ClientServiceTests
{
    private readonly ClientService _clientService;
    private readonly Mock<IClientRepository> _clientRepositoryMock;

    public ClientServiceTests()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _clientService = new ClientService(_clientRepositoryMock.Object);
    }

    [Fact]
    public async Task AddIndividualClientAsync_ShouldThrowBadRequestException_WhenPESELLengthIsInvalid()
    {
        var invalidPesel = new IndividualClientDto { PESEL = "12345" };
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _clientService.AddIndividualClientAsync(invalidPesel));
        Assert.Equal("PESEL has to consist of " + ClientConstants.PeselLength + " digits!", exception.Message);
    }

    [Fact]
    public async Task AddIndividualClientAsync_ShouldThrowBadRequestException_WhenPESELAlreadyExists()
    {
        var pesel = "12345678901";
        var individualClient = new IndividualClientDto { PESEL = pesel };

        _clientRepositoryMock.Setup(repo => repo.PeselExistsAsync(pesel)).ReturnsAsync(true);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _clientService.AddIndividualClientAsync(individualClient));
        Assert.Equal("Provided PESEL already exists!", exception.Message);
    }

    [Fact]
    public async Task AddIndividualClientAsync_ShouldAddClient_WhenPESELIsValid()
    {
        var pesel = "12345678901";
        var individualClient = new IndividualClientDto
        {
            PESEL = pesel,
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };

        _clientRepositoryMock.Setup(repo => repo.PeselExistsAsync(pesel)).ReturnsAsync(false);
        _clientRepositoryMock.Setup(repo => repo.AddClientAsync(It.IsAny<IndividualClient>())).Returns(Task.CompletedTask);
        
        await _clientService.AddIndividualClientAsync(individualClient);
        
        _clientRepositoryMock.Verify(repo => repo.AddClientAsync(It.IsAny<IndividualClient>()), Times.Once);
    }

    [Fact]
    public async Task AddCompanyClientAsync_ShouldThrowBadRequestException_WhenKRSLengthIsInvalid()
    {
        var invalidKrs = new CompanyClientDto { KRS = "12345" };
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _clientService.AddCompanyClientAsync(invalidKrs));
        Assert.Equal("KRS is " + ClientConstants.KrsLength1 + " or " + ClientConstants.KrsLength2 + " digits number!", exception.Message);
    }

    [Fact]
    public async Task AddCompanyClientAsync_ShouldThrowBadRequestException_WhenKRSAlreadyExists()
    {
        var krs = "123456789";
        var companyClient = new CompanyClientDto { KRS = krs };

        _clientRepositoryMock.Setup(repo => repo.KrsExistsAsync(krs)).ReturnsAsync(true);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _clientService.AddCompanyClientAsync(companyClient));
        Assert.Equal("Provided KRS already exists!", exception.Message);
    }

    [Fact]
    public async Task AddCompanyClientAsync_ShouldAddClient_WhenKRSIsValid()
    {
        var krs = "123456789";
        var companyClient = new CompanyClientDto
        {
            KRS = krs,
            CompanyName = "Test Company",
            Address = "456 Market St",
            Email = "info@testcompany.com",
            PhoneNumber = "987654321"
        };

        _clientRepositoryMock.Setup(repo => repo.KrsExistsAsync(krs)).ReturnsAsync(false);
        _clientRepositoryMock.Setup(repo => repo.AddClientAsync(It.IsAny<CompanyClient>())).Returns(Task.CompletedTask);
        
        await _clientService.AddCompanyClientAsync(companyClient);
        
        _clientRepositoryMock.Verify(repo => repo.AddClientAsync(It.IsAny<CompanyClient>()), Times.Once);
    }

    [Fact]
    public async Task UpdateIndividualClientAsync_ShouldThrowNotFoundException_WhenClientDoesNotExist()
    {
        var clientId = 1;
        var individualClient = new UpdateIndividualClientDto();

        _clientRepositoryMock.Setup(repo => repo.IndividualClientExistsAsync(clientId)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _clientService.UpdateIndividualClientAsync(clientId, individualClient));
        Assert.Equal("Individual client with such an ID was not found!", exception.Message);
    }

    [Fact]
    public async Task UpdateIndividualClientAsync_ShouldUpdateClient_WhenClientExists()
    {
        var clientId = 1;
        var individualClient = new UpdateIndividualClientDto();

        _clientRepositoryMock.Setup(repo => repo.IndividualClientExistsAsync(clientId)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.UpdateIndividualClientAsync(clientId, individualClient)).Returns(Task.CompletedTask);
        
        await _clientService.UpdateIndividualClientAsync(clientId, individualClient);
        
        _clientRepositoryMock.Verify(repo => repo.UpdateIndividualClientAsync(clientId, individualClient), Times.Once);
    }

    [Fact]
    public async Task UpdateCompanyClientAsync_ShouldThrowNotFoundException_WhenClientDoesNotExist()
    {
        var clientId = 1;
        var companyClient = new UpdateCompanyClientDto();

        _clientRepositoryMock.Setup(repo => repo.CompanyClientExistsAsync(clientId)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _clientService.UpdateCompanyClientAsync(clientId, companyClient));
        Assert.Equal("Company client with such an ID was not found!", exception.Message);
    }

    [Fact]
    public async Task UpdateCompanyClientAsync_ShouldUpdateClient_WhenClientExists()
    {
        var clientId = 1;
        var companyClient = new UpdateCompanyClientDto();

        _clientRepositoryMock.Setup(repo => repo.CompanyClientExistsAsync(clientId)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.UpdateCompanyClientAsync(clientId, companyClient)).Returns(Task.CompletedTask);
        
        await _clientService.UpdateCompanyClientAsync(clientId, companyClient);
        
        _clientRepositoryMock.Verify(repo => repo.UpdateCompanyClientAsync(clientId, companyClient), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteIndividualClientAsync_ShouldThrowNotFoundException_WhenClientDoesNotExist()
    {
        var clientId = 1;

        _clientRepositoryMock.Setup(repo => repo.IndividualClientExistsAsync(clientId)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _clientService.SoftDeleteIndividualClientAsync(clientId));
        Assert.Equal("Individual client with such an ID was not found!", exception.Message);
    }

    [Fact]
    public async Task SoftDeleteIndividualClientAsync_ShouldSoftDeleteClient_WhenClientExists()
    {
        var clientId = 1;

        _clientRepositoryMock.Setup(repo => repo.IndividualClientExistsAsync(clientId)).ReturnsAsync(true);
        _clientRepositoryMock.Setup(repo => repo.SoftDeleteIndividualClientAsync(clientId)).Returns(Task.CompletedTask);
        
        await _clientService.SoftDeleteIndividualClientAsync(clientId);
        
        _clientRepositoryMock.Verify(repo => repo.SoftDeleteIndividualClientAsync(clientId), Times.Once);
    }
}