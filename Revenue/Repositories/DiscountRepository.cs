using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;

namespace Revenue.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly SystemContext _systemContext;

    public DiscountRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }

    public async Task<double> GetMaxDiscountAsync(int softwareId, DateTime date)
    {
        var discount = await _systemContext.Discounts
            .Where(x => x.DateTo >= date && x.DateFrom <= date)
            .Where(x => x.Softwares.Any(s => s.IdSoftware == softwareId))
            .Select(x => (double?) x.Percentage)
            .MaxAsync();
        return discount ?? 0.0;
    }
    
}