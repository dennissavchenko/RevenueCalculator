using Revenue.Services;

namespace RevenueTest;

using Moq;
using Revenue.Entities;
using Revenue.Exceptions;
using Revenue.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class RevenueServiceTests
{
    private readonly Mock<IContactRepository> _contactRepositoryMock;
    private readonly Mock<ISubscriptionPaymentRepository> _subscriptionPaymentRepositoryMock;
    private readonly Mock<ISoftwareRepository> _softwareRepositoryMock;
    private readonly Mock<ISubscriptionClientRepository> _subscriptionClientRepositoryMock;
    private readonly RevenueService _revenueService;

    public RevenueServiceTests()
    {
        _contactRepositoryMock = new Mock<IContactRepository>();
        _subscriptionPaymentRepositoryMock = new Mock<ISubscriptionPaymentRepository>();
        _softwareRepositoryMock = new Mock<ISoftwareRepository>();
        _subscriptionClientRepositoryMock = new Mock<ISubscriptionClientRepository>();

        _revenueService = new RevenueService(
            _contactRepositoryMock.Object,
            _subscriptionPaymentRepositoryMock.Object,
            _softwareRepositoryMock.Object,
            _subscriptionClientRepositoryMock.Object,
            null!
        );
    }

    [Fact]
    public async Task GetCurrentRevenueAsync_Returns_CorrectRevenue()
    {
        // Arrange
        _contactRepositoryMock.Setup(repo => repo.GetContactsCurrentRevenueAsync()).ReturnsAsync(100);
        _subscriptionPaymentRepositoryMock.Setup(repo => repo.GetSubscriptionsCurrentRevenueAsync()).ReturnsAsync(200);

        // Act
        var result = await _revenueService.GetCurrentRevenueAsync();

        // Assert
        Assert.Equal(300, result);
    }

    [Fact]
    public async Task GetCurrentRevenueForProductAsync_ValidSoftwareId_Returns_CorrectRevenue()
    {
        // Arrange
        int softwareId = 1;
        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(softwareId)).ReturnsAsync(true);
        _contactRepositoryMock.Setup(repo => repo.GetContactsCurrentRevenueForProductAsync(softwareId)).ReturnsAsync(100);
        _subscriptionPaymentRepositoryMock.Setup(repo => repo.GetSubscriptionsCurrentRevenueForProductAsync(softwareId)).ReturnsAsync(200);

        // Act
        var result = await _revenueService.GetCurrentRevenueForProductAsync(softwareId);

        // Assert
        Assert.Equal(300, result);
    }

    [Fact]
    public async Task GetCurrentRevenueForProductAsync_NonExistingSoftwareId_ThrowsNotFoundException()
    {
        // Arrange
        int softwareId = 1;
        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(softwareId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _revenueService.GetCurrentRevenueForProductAsync(softwareId));
        Assert.Equal("Software with such an ID does not exist!", exception.Message);
    }

    [Fact]
    public async Task GetCurrentRevenueForProductInCurrencyAsync_NonExistingSoftwareId_ThrowsNotFoundException()
    {
        // Arrange
        int softwareId = 1;
        string currencyCode = "USD";
        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(softwareId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _revenueService.GetCurrentRevenueForProductInCurrencyAsync(softwareId, currencyCode));
        Assert.Equal("Software with such an ID does not exist!", exception.Message);
    }

    [Fact]
    public async Task GetPredictedRevenueForProductAsync_NonExistingSoftwareId_ThrowsNotFoundException()
    {
        // Arrange
        int softwareId = 1;
        int predictionPeriodDays = 30;
        _softwareRepositoryMock.Setup(repo => repo.SoftwareExistsAsync(softwareId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _revenueService.GetPredictedRevenueForProductAsync(softwareId, predictionPeriodDays));
        Assert.Equal("Software with such an ID does not exist!", exception.Message);
    }
}
