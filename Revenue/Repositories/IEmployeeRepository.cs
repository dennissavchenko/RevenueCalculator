using Revenue.Entities;

namespace Revenue.Repositories;

public interface IEmployeeRepository
{
    public Task<Employee?> GetEmployeeAsync(string username, string password);
}