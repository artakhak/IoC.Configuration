using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.Entities;
using IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;
using System.Collections.Generic;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.DataRepositories;

[DataRepository(databaseEntityType: typeof(Author))]
public interface IAuthorsRepository
{
    // Not we do not use RepositoryMethodMetadataForAdd  attribute in IAuthorsRepository
    // Only RepositoryMethodMetadataForAddOrUpdate
    [RepositoryMethodMetadataForAddOrUpdate]
    Author AddOrUpdateAuthor(Author author);

    [RepositoryMethodMetadataForDelete]
    void DeleteAuthor(Author author);

    [RepositoryMethodMetadataForSelect(isSelectAll: true)]
    IReadOnlyList<Author> GetAllAuthors();

    [RepositoryMethodMetadataForSelect(isSelectAll: false)]
    Author GetAuthor(long authorId);
}