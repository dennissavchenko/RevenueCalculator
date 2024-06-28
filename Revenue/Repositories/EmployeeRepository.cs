using Microsoft.EntityFrameworkCore;
using Revenue.Context;
using Revenue.Entities;

namespace Revenue.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly SystemContext _systemContext;

    public EmployeeRepository(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }

    public async Task<Employee?> GetEmployeeAsync(string username, string password)
    {
        return await _systemContext.Employees.Where(x => x.Username == username && x.Password == password).Include(x => x.EmployeeRole).SingleOrDefaultAsync();
    }
}