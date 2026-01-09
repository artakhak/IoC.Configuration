namespace WebApiDemo.Domain.Employee;

public interface IEmployeeRepository
{
    IReadOnlyList<Model.Employee> GetAllEmployees();
}
