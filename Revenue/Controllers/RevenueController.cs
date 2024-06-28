using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Revenue.Exceptions;
using Revenue.Services;

namespace Revenue.Controllers;

[ApiController]
[Route("/api/revenue")]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpGet]
    public async Task<IActionResult> GetCurrentRevenueAsync()
    {
        return Ok(await _revenueService.GetCurrentRevenueAsync());
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpGet("software/{softwareId:int}")]
    public async Task<IActionResult> GetCurrentRevenueForProductAsync(int softwareId)
    {
        try
        {
            return Ok(await _revenueService.GetCurrentRevenueForProductAsync(softwareId));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        
    }
    
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpGet("currency")]
    public async Task<IActionResult> GetCurrentRevenueInCurrencyAsync([FromQuery] string currencyCode)
    {
        try
        {
            return Ok(await _revenueService.GetCurrentRevenueInCurrencyAsync(currencyCode));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpGet("software/{softwareId:int}/currency")]
    public async Task<IActionResult> GetCurrentRevenueForProductInCurrencyAsync(int softwareId, [FromQuery] string currencyCode)
    {
        try
        {
            return Ok(await _revenueService.GetCurrentRevenueForProductInCurrencyAsync(softwareId, currencyCode));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpGet("predicted")]
    public async Task<IActionResult> GetPredictedRevenueAsync([FromQuery] int predictionPeriodDays)
    {
        return Ok(await _revenueService.GetPredictedRevenueAsync(predictionPeriodDays));
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpGet("predicted/software/{softwareId:int}")]
    public async Task<IActionResult> GetPredictedRevenueForProductAsync(int softwareId, [FromQuery] int predictionPeriodDays)
    {
        try
        {
            return Ok(await _revenueService.GetPredictedRevenueForProductAsync(softwareId, predictionPeriodDays));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        
    }
    
}