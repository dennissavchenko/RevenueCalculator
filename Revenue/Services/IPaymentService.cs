using Revenue.DTOs;
using Revenue.Entities;

namespace Revenue.Services;

public interface IPaymentService
{
    public Task IssuePaymentAsync(PaymentDto payment);
    
}