using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;
using Revenue.Enums;

namespace Revenue.Repositories;

public class ContractPaymentRepository : IContractPaymentRepository
{
    private readonly SystemContext _systemContext;

    public ContractPaymentRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }
    
    public async Task AddContractPaymentAsync(ContractPayment payment)
    {
        await _systemContext.ContractPayments.AddAsync(payment);
        await _systemContext.SaveChangesAsync();
    }

    public async Task ReturnContractPaymentsAsync(int contractId)
    {
        var payments = await _systemContext.ContractPayments.Where(x => x.IdContract == contractId)
            .Include(payment => payment.PaymentStatus).ToListAsync();
        foreach (var payment in payments)
        {
            payment.IdPaymentStatus = (int) PaymentStatuses.Returned;
        }
        await _systemContext.SaveChangesAsync();
    }

    public async Task<double> GetContractPaymentsSumAsync(int contractId)
    {
        return await _systemContext.ContractPayments.Where(x => x.IdContract == contractId).SumAsync(x => x.Amount);
    }
    
    
}