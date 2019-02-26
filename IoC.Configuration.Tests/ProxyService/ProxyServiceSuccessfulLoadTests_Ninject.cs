using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ProxyService
{
    [TestClass]
    public class ProxyServiceSuccessfulLoadTests_Ninject : ProxyServiceSuccessfulLoadTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            OnClassInitialize(DiImplementationType.Ninject, ProxyServiceConfigurationRelativePath);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}