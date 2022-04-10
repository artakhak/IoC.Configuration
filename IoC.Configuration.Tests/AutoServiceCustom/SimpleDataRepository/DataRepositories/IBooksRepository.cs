using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.Entities;
using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;
using System.Collections.Generic;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.DataRepositories;

[DataRepository(databaseEntityType: typeof(Book))]
public interface IBooksRepository
{
    // Not we do not use RepositoryMethodMetadataForAdd  attribute in IBooksRepository
    // Only RepositoryMethodMetadataForAddOrUpdate
    [RepositoryMethodMetadataForAddOrUpdate]
    Book AddOrUpdateBook(Book book);

    [RepositoryMethodMetadataForDelete]
    void DeleteBook(Book book);

    [RepositoryMethodMetadataForSelect(isSelectAll: true)]
    IReadOnlyList<Book> GetAllBooks();

    [RepositoryMethodMetadataForSelect(isSelectAll: false)]
    Book GetBook(long bookId);
}