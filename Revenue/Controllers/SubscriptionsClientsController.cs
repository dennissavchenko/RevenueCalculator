using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Revenue.DTOs;
using Revenue.Exceptions;
using Revenue.Services;

namespace Revenue.Controllers;

[ApiController]
[Route("/api/subscriptionsClients")]
public class SubscriptionsClientsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsClientsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpPost]
    public async Task<IActionResult> BuySubscriptionAsync([FromBody] BuySubscriptionDto buySubscription)
    {
        try
        {
            await _subscriptionService.BuySubscriptionAsync(buySubscription);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
}