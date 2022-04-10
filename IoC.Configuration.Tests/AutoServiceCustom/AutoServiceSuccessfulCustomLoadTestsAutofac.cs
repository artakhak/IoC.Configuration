using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.AutoServiceCustom;

[TestFixture]
public class AutoServiceSuccessfulCustomLoadTestsAutofac : AutoServiceCustomSuccessfulLoadTests
{
    [OneTimeSetUp]
    public static void ClassInitialize()
    {
        OnClassInitialize(DiImplementationType.Autofac);
    }

    [OneTimeTearDown]
    public static void ClassCleanup()
    {
        OnClassCleanup();
    }
}