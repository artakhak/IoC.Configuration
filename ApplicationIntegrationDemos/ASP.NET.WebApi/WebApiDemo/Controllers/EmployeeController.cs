using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Domain.Employee;
using WebApiDemo.Domain.Employee.Model;

namespace WebApiDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _companyRepository;

    public EmployeeController(IEmployeeRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    [HttpGet("all", Name = "all-employees")]
    public IEnumerable<Employee> GetAllEmployees()
    {
        return _companyRepository.GetAllEmployees();
    }
}
