using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Extension.Domain.Company;
using WebApiDemo.Extension.Domain.Company.Model;

namespace WebApiDemo.Extension.Controllers;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyController(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }
    
    [HttpGet("all", Name = "all-companies")]
    public IEnumerable<Company> GetAllCompanies()
    {
        return _companyRepository.GetAllCompanies();
    }
}
