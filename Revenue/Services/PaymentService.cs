using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Enums;
using Revenue.Exceptions;
using Revenue.Repositories;

namespace Revenue.Services;

public class PaymentService : IPaymentService
{
    
    private readonly IContactRepository _contactRepository;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IContactRepository contactRepository, IPaymentRepository paymentRepository)
    {
        _contactRepository = contactRepository;
        _paymentRepository = paymentRepository;
    }
    
    public async Task IssuePaymentAsync(PaymentDto payment)
    {
        if (!await _contactRepository.ContractExistsAsync(payment.IdContract))
        {
            throw new BadRequestException("Contract with such an ID does not exist or was cancelled!");
        }

        var contract = await _contactRepository.GetContractAsync(payment.IdContract);

        if (contract.IsSigned)
        {
            throw new BadRequestException("Contract with such an ID is already payed and signed!");
        }

        double sum = await _paymentRepository.GetPaymentsSumAsync(payment.IdContract);

        if (payment.Date > contract.EndDate.Date)
        {
            await _contactRepository.CancelContractAsync(payment.IdContract);
            await _paymentRepository.ReturnPaymentsAsync(payment.IdContract);
            throw new BadRequestException("Contract was outdated. Now it is going to be cancelled. All your previous payments will be returned!");
        }

        if (payment.Amount + sum > contract.Price)
        {
            throw new BadRequestException("Amount issued is too big. You only need to pay " + (contract.Price - sum) + " more!");
        }

        var newPayment = new ContractPayment
        {
            Amount = payment.Amount,
            IdContract = payment.IdContract,
            Date = payment.Date,
            IdPaymentStatus = (int) PaymentStatuses.Accepted
        };

        if ((payment.Amount + sum).Equals(contract.Price))
        {
            await _paymentRepository.IssuePaymentAsync(newPayment);
            await _contactRepository.SignContractAsync(payment.IdContract);
        }

        if (payment.Amount + sum < contract.Price)
        {
            await _paymentRepository.IssuePaymentAsync(newPayment);
        }

    }
}