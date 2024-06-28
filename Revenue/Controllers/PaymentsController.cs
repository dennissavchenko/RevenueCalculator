using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Revenue.DTOs;
using Revenue.Exceptions;
using Revenue.Services;

namespace Revenue.Controllers;

[ApiController]
[Route("/api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpPost("contracts")]
    public async Task<IActionResult> IssueContractPaymentAsync([FromBody] ContractPaymentDto contractPayment)
    {
        try
        {
            await _paymentService.IssueContractPaymentAsync(contractPayment);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpPost("subscriptions")]
    public async Task<IActionResult> IssueSubscriptionPaymentAsync([FromBody] SubscriptionPaymentDto subscriptionPayment)
    {
        try
        {
            await _paymentService.IssueSubscriptionPaymentAsync(subscriptionPayment);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
}