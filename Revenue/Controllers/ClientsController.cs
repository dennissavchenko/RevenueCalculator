using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Revenue.DTOs;
using Revenue.Services;
using Revenue.Exceptions;

namespace Revenue.Controllers;

[ApiController]
[Route("/api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpPost("individuals")]
    public async Task<IActionResult> AddIndividualClientAsync([FromBody] IndividualClientDto individualClient)
    {
        try
        {
            await _clientService.AddIndividualClientAsync(individualClient);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [Authorize(Policy = "StandardPolicy")]
    [HttpPost("companies")]
    public async Task<IActionResult> AddCompanyClientAsync([FromBody] CompanyClientDto companyClient)
    {
        try
        {
            await _clientService.AddCompanyClientAsync(companyClient);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("individuals/{clientId:int}")]
    public async Task<IActionResult> UpdateIndividualClientAsync(int clientId, [FromBody] UpdateIndividualClientDto individualClient)
    {
        try
        {
            await _clientService.UpdateIndividualClientAsync(clientId, individualClient);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("companies/{clientId:int}")]
    public async Task<IActionResult> UpdateCompanyClientAsync(int clientId, [FromBody] UpdateCompanyClientDto companyClient)
    {
        try
        {
            await _clientService.UpdateCompanyClientAsync(clientId, companyClient);
            return NoContent();
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("individuals/{clientId:int}")]
    public async Task<IActionResult> SoftDeleteIndividualClientAsync(int clientId)
    {
        try
        {
            await _clientService.SoftDeleteIndividualClientAsync(clientId);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    } 
    
}