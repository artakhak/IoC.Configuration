using System;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;

[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = false)]
public class RepositoryMethodMetadataForSelectAttribute : Attribute
{
    public RepositoryMethodMetadataForSelectAttribute(bool isSelectAll)
    {
        IsSelectAll = isSelectAll;
    }

    public bool IsSelectAll { get; }
}