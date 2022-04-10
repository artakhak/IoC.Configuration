using System;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes;

[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = false)]
public class RepositoryMethodMetadataForAddAttribute : Attribute
{
}