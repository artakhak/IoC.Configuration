using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.AttributeValueTransformers;

[TestFixture]
public class AttributeValueTransformersTests_Ninject : AttributeValueTransformersTests
{
    [SetUp]
    public static void OnTestInitialize()
    {
        OnTestInitialize(DiImplementationType.Ninject);
    }
}