namespace Revenue.Entities;

public class EmployeeRole
{
    public int IdEmployeeRole { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}