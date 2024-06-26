using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;
using Revenue.Enums;

namespace Revenue.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly SystemContext _systemContext;

    public PaymentRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }
    
    public async Task IssuePaymentAsync(ContractPayment payment)
    {
        await _systemContext.ContractPayments.AddAsync(payment);
        await _systemContext.SaveChangesAsync();
    }

    public async Task ReturnPaymentsAsync(int contractId)
    {
        var payments = await _systemContext.ContractPayments.Where(x => x.IdContract == contractId)
            .Include(payment => payment.PaymentStatus).ToListAsync();
        foreach (var payment in payments)
        {
            payment.IdPaymentStatus = (int) PaymentStatuses.Returned;
        }
        await _systemContext.SaveChangesAsync();
    }

    public async Task<double> GetPaymentsSumAsync(int contractId)
    {
        return await _systemContext.ContractPayments.Where(x => x.IdContract == contractId).SumAsync(x => x.Amount);
    }
    
    
}