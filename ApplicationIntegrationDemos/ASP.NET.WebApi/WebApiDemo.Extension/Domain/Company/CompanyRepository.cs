namespace WebApiDemo.Extension.Domain.Company;

public class CompanyRepository : ICompanyRepository
{
    /// <inheritdoc />
    public IReadOnlyList<Model.Company> GetAllCompanies()
    {
        return new List<Model.Company>
        {
            new Model.Company {Name = "Strange Things, Inc", CEO = "John Malkowich"},
            new Model.Company {Name = "Sherwood Forest Timber, Inc", CEO = "Robin Wood"}
        };
    }
}
