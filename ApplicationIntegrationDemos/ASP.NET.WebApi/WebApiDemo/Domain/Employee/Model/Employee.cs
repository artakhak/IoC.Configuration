namespace WebApiDemo.Domain.Employee.Model;

public class Employee
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Salary { get; set; }
}
