using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.AutoServiceCustom;

[TestFixture]
public class AutoServiceSuccessfulCustomLoadTestsNinject : AutoServiceCustomSuccessfulLoadTests
{
    [OneTimeSetUp]
    public static void ClassInitialize()
    {
        OnClassInitialize(DiImplementationType.Ninject);
    }

    [OneTimeTearDown]
    public static void ClassCleanup()
    {
        OnClassCleanup();
    }
}