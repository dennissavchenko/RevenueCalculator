namespace Revenue.Entities;

public class Employee
{
    public int IdEmployee { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int IdEmployeeRole { get; set; }
    public EmployeeRole EmployeeRole { get; set; }
}