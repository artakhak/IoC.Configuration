using System;
using JetBrains.Annotations;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class DatabaseEntityAttribute : Attribute
{
    public DatabaseEntityAttribute([NotNull] string tableName)
    {
        TableName = tableName;
    }

    [NotNull]
    public string TableName { get; }
}