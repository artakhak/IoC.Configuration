using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.AutoService
{
    [TestClass]
    public class AutoServiceSuccessfulLoadTestsNinject : AutoServiceSuccessfulLoadTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            OnClassInitialize(DiImplementationType.Ninject, AutoServiceConfigurationRelativePath);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}