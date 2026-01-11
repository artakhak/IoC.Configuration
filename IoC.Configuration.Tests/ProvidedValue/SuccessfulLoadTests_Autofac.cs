using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ProvidedValue
{
    [TestFixture]
    public class SuccessfulLoadTests_Autofac : SuccessfulLoadTests
    {
        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            OnClassInitialize(DiImplementationType.Autofac, ConfigurationRelativePath);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}