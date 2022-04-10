using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.AttributeValueTransformers;

[TestFixture]
public class AttributeValueTransformersTests_Autofac : AttributeValueTransformersTests
{
    [SetUp]
    public static void OnTestInitialize()
    {
        OnTestInitialize(DiImplementationType.Autofac);
    }
}