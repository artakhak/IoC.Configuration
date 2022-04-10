using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ValueImplementation
{
    [TestFixture]
    public class ValueImplementationSuccessfulLoadTests_Autofac : ValueImplementationSuccessfulLoadTests
    {
        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            OnClassInitialize(DiImplementationType.Autofac, ValueImplementationConfigurationRelativePath);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}