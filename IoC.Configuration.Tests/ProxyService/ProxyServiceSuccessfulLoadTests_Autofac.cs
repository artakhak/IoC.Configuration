using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ProxyService
{
    [TestClass]
    public class ProxyServiceSuccessfulLoadTests_Autofac : ProxyServiceSuccessfulLoadTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            OnClassInitialize(DiImplementationType.Autofac, ProxyServiceConfigurationRelativePath);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}