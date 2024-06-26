using Microsoft.AspNetCore.Mvc;
using Revenue.DTOs;
using Revenue.Services;

namespace Revenue.Controllers;

[ApiController]
[Route("/api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost]
    public async Task<IActionResult> IssuePaymentAsync([FromBody] PaymentDto payment)
    {
        try
        {
            await _paymentService.IssuePaymentAsync(payment);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}