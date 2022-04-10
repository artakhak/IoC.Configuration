using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.Entities;

[DatabaseEntity(tableName:nameof(Author))]
public class Author
{
    [Column(databaseType: "BIGINT", isKeyAttribute:true)]
    public long? AuthorId { get; set; }

    [Column(databaseType: "VARCHAR(50)")]

    public string FirstName { get; set; }

    [Column(databaseType: "VARCHAR(50)")]

    public string LastName { get; set; }

    [Column(databaseType: "VARCHAR(200)", isRequired:false)]
    public string Address { get; set; }
}