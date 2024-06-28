namespace Revenue.Services;

public interface IRevenueService
{
    public Task<double> GetCurrentRevenueAsync();
    public Task<double> GetCurrentRevenueForProductAsync(int softwareId);
    public Task<double> GetCurrentRevenueInCurrencyAsync(string currencyCode);
    public Task<double> GetCurrentRevenueForProductInCurrencyAsync(int softwareId, string currencyCode);
    public Task<double> GetPredictedRevenueAsync(int predictionPeriodDays);
    public Task<double> GetPredictedRevenueForProductAsync(int softwareId, int predictionPeriodDays);
}