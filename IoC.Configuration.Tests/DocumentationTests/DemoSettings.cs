using IoC.Configuration.DiContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using OROptimizer.Diagnostics.Log;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.DocumentationTests
{
    [TestClass]
    public class DemoSettings
    {
        [TestInitialize]
        public void TestInitialize()
        {
            if (!LogHelper.IsContextInitialized)
                LogHelper.RegisterContext(new LogHelper4TestsContext());
        }

        [TestMethod]
        public void TestSettings()
        {
            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()
                  .StartFileBasedDi(
                                new FileBasedConfigurationFileContentsProvider(
                                    Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration1.xml")),
                                    Helpers.TestsEntryAssemblyFolder,
                                    (sender, e) => { })
                    .WithoutPresetDiContainer()
                    .AddAdditionalDiModules(new TestDiModule())
                    .RegisterModules()
                    .Start())
            {
                var diContainer = containerInfo.DiContainer;

                Assert.IsNotNull(containerInfo.DiContainer.Resolve<TestInjectedSettings>());
            }
        }

        private class TestDiModule : IoC.Configuration.DiContainer.ModuleAbstr
        {
            protected override void AddServiceRegistrations()
            {
                this.Bind<TestInjectedSettings>().ToSelf();
            }
        }

        public class TestInjectedSettings
        {
            public TestInjectedSettings(ISettings settings)
            {
                Assert.IsTrue(settings.GetSettingValue<double>("MaxCharge", 5.3, 
                                            out var maxChargeSettingValue));
                Assert.AreEqual(155.7, maxChargeSettingValue);
                
                Assert.IsFalse(settings.GetSettingValue<int>("MaxCharge", 5, 
                                            out var settingValueNotFound_InvalidType));
                Assert.AreEqual(5, settingValueNotFound_InvalidType);

                Assert.IsFalse(settings.GetSettingValue<int>("MaxChargeInvalid", 7, 
                                            out var nonExistentSettingValue));
                Assert.AreEqual(7, nonExistentSettingValue);

                try
                {
                    // This call will throw an exception, since there is no setting of double
                    // type with name "MaxChargeInvalid".
                    settings.GetSettingValueOrThrow<double>("MaxChargeInvalid");
                    Assert.Fail("An exception should have been thrown.");
                }
                catch
                {
                }
            }
        }
    }
}
