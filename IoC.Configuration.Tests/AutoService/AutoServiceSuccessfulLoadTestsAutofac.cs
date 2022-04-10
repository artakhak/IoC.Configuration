using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.AutoService
{
    [TestFixture]
    public class AutoServiceSuccessfulLoadTestsAutofac : AutoServiceCustomSuccessfulLoadTests
    {
        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            OnClassInitialize(DiImplementationType.Autofac, AutoServiceConfigurationRelativePath);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}