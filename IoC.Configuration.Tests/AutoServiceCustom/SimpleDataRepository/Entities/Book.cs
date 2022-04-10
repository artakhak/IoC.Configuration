using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.Entities;

[DatabaseEntity(tableName: nameof(Book))]
public class Book
{
    [Column(databaseType: "BIGINT", isKeyAttribute: true)]
    public long? BookId { get; set; }

    [Column(databaseType: "VARCHAR(200)")]
    public string Title { get; set; }
}