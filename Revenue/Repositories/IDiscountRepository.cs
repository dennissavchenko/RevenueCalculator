namespace Revenue.Repositories;

public interface IDiscountRepository
{
    public Task<double> GetMaxDiscountAsync(int softwareId, DateTime dateFrom);
}