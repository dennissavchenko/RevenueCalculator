using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Revenue.DTOs;
using Revenue.Entities;
using Revenue.Repositories;

namespace Revenue.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IEmployeeRepository _employeeRepository;

    public AuthController(IConfiguration configuration, IEmployeeRepository employeeRepository)
    {
        _configuration = configuration;
        _employeeRepository = employeeRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var employee = await _employeeRepository.GetEmployeeAsync(login.Username, login.Password);
        if (employee == null)
        {
            return Unauthorized();
        }
        var token = GenerateJwtToken(employee, _configuration);
        return Ok(new { token });
    }

    private string GenerateJwtToken(Employee employee, IConfiguration configuration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"] ?? string.Empty);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Name, employee.Username),
                new(ClaimTypes.Role, employee.EmployeeRole.Name)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Audience = configuration["Jwt:Audience"],
            Issuer = configuration["Jwt:Issuer"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        Console.Write(employee.Username + " " + employee.EmployeeRole.Name);
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}