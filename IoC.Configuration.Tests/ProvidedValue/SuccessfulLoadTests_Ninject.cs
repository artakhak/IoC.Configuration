using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ProvidedValue
{
    [TestFixture]
    public class SuccessfulLoadTests_Ninject : SuccessfulLoadTests
    {
        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            OnClassInitialize(DiImplementationType.Ninject, ConfigurationRelativePath);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}