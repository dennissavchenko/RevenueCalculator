using Moq;
using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Exceptions;
using Revenue.Repositories;
using Revenue.Services;

namespace RevenueTest;

public class PaymentServiceTests
{
    private readonly Mock<IContactRepository> _contactRepositoryMock;
    private readonly Mock<IContractPaymentRepository> _contractPaymentRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ISubscriptionClientRepository> _subscriptionClientRepositoryMock;
    private readonly Mock<ISubscriptionPaymentRepository> _subscriptionPaymentRepositoryMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _contactRepositoryMock = new Mock<IContactRepository>();
        _contractPaymentRepositoryMock = new Mock<IContractPaymentRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _subscriptionClientRepositoryMock = new Mock<ISubscriptionClientRepository>();
        _subscriptionPaymentRepositoryMock = new Mock<ISubscriptionPaymentRepository>();
        
        _paymentService = new PaymentService(
            _contactRepositoryMock.Object,
            _contractPaymentRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _subscriptionRepositoryMock.Object,
            _subscriptionClientRepositoryMock.Object,
            _subscriptionPaymentRepositoryMock.Object
        );
    }

    [Fact]
    public async Task IssueContractPaymentAsync_ValidPayment_AddsPayment()
    {
        var contractPaymentDto = new ContractPaymentDto
        {
            IdContract = 1,
            Amount = 100,
            Date = DateTime.Today
        };

        var contract = new Contract
        {
            IdContract = contractPaymentDto.IdContract,
            IsSigned = false,
            EndDate = DateTime.Today.AddDays(30),
            Price = 100
        };

        _contactRepositoryMock.Setup(repo => repo.ContractExistsAsync(contractPaymentDto.IdContract)).ReturnsAsync(true);
        _contactRepositoryMock.Setup(repo => repo.GetContractAsync(contractPaymentDto.IdContract)).ReturnsAsync(contract);
        _contractPaymentRepositoryMock.Setup(repo => repo.GetContractPaymentsSumAsync(contractPaymentDto.IdContract)).ReturnsAsync(0);

        await _paymentService.IssueContractPaymentAsync(contractPaymentDto);
        
        _contractPaymentRepositoryMock.Verify(repo => repo.AddContractPaymentAsync(It.IsAny<ContractPayment>()), Times.Once);
        _contactRepositoryMock.Verify(repo => repo.SignContractAsync(contractPaymentDto.IdContract), Times.Once);
    }

    [Fact]
    public async Task IssueContractPaymentAsync_ContractDoesNotExist_ThrowsBadRequestException()
    {
        var contractPaymentDto = new ContractPaymentDto
        {
            IdContract = 1,
            Amount = 100,
            Date = DateTime.Today
        };

        _contactRepositoryMock.Setup(repo => repo.ContractExistsAsync(contractPaymentDto.IdContract)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _paymentService.IssueContractPaymentAsync(contractPaymentDto));
        Assert.Equal("Contract with such an ID does not exist or was cancelled!", exception.Message);
    }

    [Fact]
    public async Task IssueContractPaymentAsync_AmountExceedsContractPrice_ThrowsBadRequestException()
    {
        var contractPaymentDto = new ContractPaymentDto
        {
            IdContract = 1,
            Amount = 200,
            Date = DateTime.Today
        };

        var contract = new Contract
        {
            IdContract = contractPaymentDto.IdContract,
            IsSigned = false,
            EndDate = DateTime.Today.AddDays(30),
            Price = 150
        };

        _contactRepositoryMock.Setup(repo => repo.ContractExistsAsync(contractPaymentDto.IdContract)).ReturnsAsync(true);
        _contactRepositoryMock.Setup(repo => repo.GetContractAsync(contractPaymentDto.IdContract)).ReturnsAsync(contract);
        _contractPaymentRepositoryMock.Setup(repo => repo.GetContractPaymentsSumAsync(contractPaymentDto.IdContract)).ReturnsAsync(50);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _paymentService.IssueContractPaymentAsync(contractPaymentDto));
        Assert.Equal("Amount issued is too big. You only need to pay 100 more!", exception.Message);
    }

    [Fact]
    public async Task IssueContractPaymentAsync_ContractAlreadySigned_ThrowsBadRequestException()
    {
        var contractPaymentDto = new ContractPaymentDto
        {
            IdContract = 1,
            Amount = 100,
            Date = DateTime.Today
        };

        var contract = new Contract
        {
            IdContract = contractPaymentDto.IdContract,
            IsSigned = true,
            EndDate = DateTime.Today.AddDays(30),
            Price = 100
        };

        _contactRepositoryMock.Setup(repo => repo.ContractExistsAsync(contractPaymentDto.IdContract)).ReturnsAsync(true);
        _contactRepositoryMock.Setup(repo => repo.GetContractAsync(contractPaymentDto.IdContract)).ReturnsAsync(contract);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _paymentService.IssueContractPaymentAsync(contractPaymentDto));
        Assert.Equal("Contract with such an ID is already payed and signed!", exception.Message);
    }

    [Fact]
    public async Task IssueSubscriptionPaymentAsync_ValidPayment_AddsPayment()
    {
        var subscriptionPaymentDto = new SubscriptionPaymentDto
        {
            IdClient = 1,
            IdSubscription = 1,
            Amount = 100,
            Date = DateTime.Today
        };

        var subscriptionClient = new SubscriptionClient
        {
            IdClient = subscriptionPaymentDto.IdClient,
            IdSubscription = subscriptionPaymentDto.IdSubscription,
            Price = 100,
            NextPaymentDate = DateTime.Today
        };

        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(subscriptionPaymentDto.IdClient)).ReturnsAsync(true);
        _subscriptionRepositoryMock.Setup(repo => repo.SubscriptionExistsAsync(subscriptionPaymentDto.IdSubscription)).ReturnsAsync(true);
        _subscriptionClientRepositoryMock.Setup(repo => repo.ClientAlreadySubscribedAsync(subscriptionPaymentDto.IdClient, subscriptionPaymentDto.IdSubscription)).ReturnsAsync(true);
        _subscriptionClientRepositoryMock.Setup(repo => repo.GetSubscriptionClientAsync(subscriptionPaymentDto.IdClient, subscriptionPaymentDto.IdSubscription)).ReturnsAsync(subscriptionClient);
        
        await _paymentService.IssueSubscriptionPaymentAsync(subscriptionPaymentDto);
        
        _subscriptionPaymentRepositoryMock.Verify(repo => repo.AddSubscriptionPaymentAsync(It.IsAny<SubscriptionPayment>()), Times.Once);
        _subscriptionClientRepositoryMock.Verify(repo => repo.UpgradeNextRenewalDateAsync(subscriptionPaymentDto.IdClient, subscriptionPaymentDto.IdSubscription), Times.Once);
    }

    [Fact]
    public async Task IssueSubscriptionPaymentAsync_ClientNotFound_ThrowsBadRequestException()
    {
        var subscriptionPaymentDto = new SubscriptionPaymentDto
        {
            IdClient = 1,
            IdSubscription = 1,
            Amount = 100,
            Date = DateTime.Today
        };

        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(subscriptionPaymentDto.IdClient)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _paymentService.IssueSubscriptionPaymentAsync(subscriptionPaymentDto));
        Assert.Equal("Client with such an ID was not found!", exception.Message);
    }

    [Fact]
    public async Task IssueSubscriptionPaymentAsync_SubscriptionNotFound_ThrowsBadRequestException()
    {
        var subscriptionPaymentDto = new SubscriptionPaymentDto
        {
            IdClient = 1,
            IdSubscription = 1,
            Amount = 100,
            Date = DateTime.Today
        };

        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(subscriptionPaymentDto.IdClient)).ReturnsAsync(true);
        _subscriptionRepositoryMock.Setup(repo => repo.SubscriptionExistsAsync(subscriptionPaymentDto.IdSubscription)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _paymentService.IssueSubscriptionPaymentAsync(subscriptionPaymentDto));
        Assert.Equal("Subscription with such an ID was not found!", exception.Message);
    }

    [Fact]
    public async Task IssueSubscriptionPaymentAsync_ClientNotSubscribed_ThrowsBadRequestException()
    {
        var subscriptionPaymentDto = new SubscriptionPaymentDto
        {
            IdClient = 1,
            IdSubscription = 1,
            Amount = 100,
            Date = DateTime.Today
        };

        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(subscriptionPaymentDto.IdClient)).ReturnsAsync(true);
        _subscriptionRepositoryMock.Setup(repo => repo.SubscriptionExistsAsync(subscriptionPaymentDto.IdSubscription)).ReturnsAsync(true);
        _subscriptionClientRepositoryMock.Setup(repo => repo.ClientAlreadySubscribedAsync(subscriptionPaymentDto.IdClient, subscriptionPaymentDto.IdSubscription)).ReturnsAsync(false);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _paymentService.IssueSubscriptionPaymentAsync(subscriptionPaymentDto));
        Assert.Equal("Client is not subscribed to the subscription!", exception.Message);
    }

    [Fact]
    public async Task IssueSubscriptionPaymentAsync_PaymentLate_CancelsSubscription()
    {
        var subscriptionPaymentDto = new SubscriptionPaymentDto
        {
            IdClient = 1,
            IdSubscription = 1,
            Amount = 100,
            Date = DateTime.Today.AddDays(2)
        };

        var subscriptionClient = new SubscriptionClient
        {
            IdClient = subscriptionPaymentDto.IdClient,
            IdSubscription = subscriptionPaymentDto.IdSubscription,
            Price = 100,
            NextPaymentDate = DateTime.Today
        };

        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(subscriptionPaymentDto.IdClient)).ReturnsAsync(true);
        _subscriptionRepositoryMock.Setup(repo => repo.SubscriptionExistsAsync(subscriptionPaymentDto.IdSubscription)).ReturnsAsync(true);
        _subscriptionClientRepositoryMock.Setup(repo => repo.ClientAlreadySubscribedAsync(subscriptionPaymentDto.IdClient, subscriptionPaymentDto.IdSubscription)).ReturnsAsync(true);
        _subscriptionClientRepositoryMock.Setup(repo => repo.GetSubscriptionClientAsync(subscriptionPaymentDto.IdClient, subscriptionPaymentDto.IdSubscription)).ReturnsAsync(subscriptionClient);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _paymentService.IssueSubscriptionPaymentAsync(subscriptionPaymentDto));
        Assert.Equal("Payment is late. We have to cancel the client's subscriptions!", exception.Message);
    }

    [Fact]
    public async Task IssueSubscriptionPaymentAsync_AlreadyPaidThisPeriod_ThrowsBadRequestException()
    {
        var subscriptionPaymentDto = new SubscriptionPaymentDto
        {
            IdClient = 1,
            IdSubscription = 1,
            Amount = 100,
            Date = DateTime.Today
        };

        var subscriptionClient = new SubscriptionClient
        {
            IdClient = subscriptionPaymentDto.IdClient,
            IdSubscription = subscriptionPaymentDto.IdSubscription,
            Price = 100,
            NextPaymentDate = DateTime.Today.AddDays(1)
        };

        _clientRepositoryMock.Setup(repo => repo.ClientExistsAsync(subscriptionPaymentDto.IdClient)).ReturnsAsync(true);
        _subscriptionRepositoryMock.Setup(repo => repo.SubscriptionExistsAsync(subscriptionPaymentDto.IdSubscription)).ReturnsAsync(true);
        _subscriptionClientRepositoryMock.Setup(repo => repo.ClientAlreadySubscribedAsync(subscriptionPaymentDto.IdClient, subscriptionPaymentDto.IdSubscription)).ReturnsAsync(true);
        _subscriptionClientRepositoryMock.Setup(repo => repo.GetSubscriptionClientAsync(subscriptionPaymentDto.IdClient, subscriptionPaymentDto.IdSubscription)).ReturnsAsync(subscriptionClient);
        
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _paymentService.IssueSubscriptionPaymentAsync(subscriptionPaymentDto));
        Assert.Equal("You have already payed for your subscription this period!", exception.Message);
    }
}