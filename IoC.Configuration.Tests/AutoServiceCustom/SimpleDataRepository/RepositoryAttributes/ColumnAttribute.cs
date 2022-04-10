using System;
using JetBrains.Annotations;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public ColumnAttribute([NotNull] string databaseType, [CanBeNull] string name = null,
        bool isRequired = true,
        bool isKeyAttribute = false)
    {
        DatabaseType = databaseType;
        Name = name;
        IsRequired = isRequired;
        IsKeyAttribute = isKeyAttribute;
    }

    [NotNull] public string DatabaseType { get; }

    [CanBeNull]
    public string Name { get; }

    public bool IsRequired { get; }
    public bool IsKeyAttribute { get; }
}