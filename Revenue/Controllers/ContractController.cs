using Microsoft.AspNetCore.Mvc;
using Revenue.DTOs;
using Revenue.Services;

namespace Revenue.Controllers;

[ApiController]
[Route("/api/contracts")]
public class ContractController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractController(IContractService contractService)
    {
        _contractService = contractService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContractAsync([FromBody] ContractDto contract)
    {
        try
        {
            await _contractService.CreateContractAsync(contract);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}