namespace WebApiDemo.Domain.Employee;

public class EmployeeRepository : IEmployeeRepository
{
    /// <inheritdoc />
    public IReadOnlyList<Model.Employee> GetAllEmployees()
    {
        return new List<Model.Employee>
        {
            new Model.Employee
            {
                Id = "100000001",
                FirstName = "John",
                LastName = "Smith",
                Salary = 100000
            },
            new Model.Employee
            {
                Id = "100000002",
                FirstName = "Alice",
                LastName = "Alice",
                Salary = 200000
            }
        };
    }
}
