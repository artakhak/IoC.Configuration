using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.SettingValue
{
    [TestClass]
    public class SettingValueSuccessfulLoadTests_Ninject : SettingValueSuccessfulLoadTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            OnClassInitialize(DiImplementationType.Ninject, SettingValueConfigurationRelativePath);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}