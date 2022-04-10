using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.Entities;

[DatabaseEntity(tableName: nameof(AuthorBook))]
public class AuthorBook
{
    [Column(databaseType: "BIGINT", isKeyAttribute: true)]
    public long? AuthorBookId { get; set; }

    [Column(databaseType: "BIGINT")]
    public long AuthorId { get; set; }

    [Column(databaseType: "BIGINT")]
    public long BookId { get; set; }
}