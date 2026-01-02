namespace WebApiDemo.Extension.Domain.Company;

public interface ICompanyRepository
{
    IReadOnlyList<Model.Company> GetAllCompanies();
}