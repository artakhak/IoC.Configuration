using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.Entities;
using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;
using System.Collections.Generic;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.DataRepositories;

[DataRepository(databaseEntityType: typeof(AuthorBook))]
public interface IAuthorBooksRepository
{
    // Not we do not use RepositoryMethodMetadataForAddOrUpdate attribute in IAuthorBooksRepository
    // Only RepositoryMethodMetadataForAdd
    [RepositoryMethodMetadataForAdd]
    AuthorBook AddAuthorBook(AuthorBook authorBook);

    [RepositoryMethodMetadataForDelete]
    void DeleteAuthorBook(AuthorBook authorBook);

    [RepositoryMethodMetadataForSelect(isSelectAll: true)]
    IReadOnlyList<AuthorBook> GetAllAuthorBooks();

    // We do not have to include all methods supported by repository method attributes.
    // This repository interface demonstrates the case when we do not add a method supported by
    // [RepositoryMethodMetadataForSelect(isSelectAll: false)] attribute.
    // [RepositoryMethodMetadataForSelect(isSelectAll: false)]
    // AuthorBook GetAuthorBook(long authorBookId);
}