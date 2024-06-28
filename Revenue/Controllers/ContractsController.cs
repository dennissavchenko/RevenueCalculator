using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Revenue.DTOs;
using Revenue.Exceptions;
using Revenue.Services;

namespace Revenue.Controllers;

[ApiController]
[Route("/api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpPost]
    public async Task<IActionResult> CreateContractAsync([FromBody] ContractDto contract)
    {
        try
        {
            await _contractService.CreateContractAsync(contract);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
}