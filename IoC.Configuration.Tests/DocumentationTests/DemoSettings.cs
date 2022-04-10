using OROptimizer.Diagnostics.Log;
using System.IO;
using IoC.Configuration.DiContainerBuilder.FileBased;
using NUnit.Framework;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.DocumentationTests
{
    [TestFixture]
    public class DemoSettings
    {
        [SetUp]
        public void TestInitialize()
        {
            if (!LogHelper.IsContextInitialized)
                LogHelper.RegisterContext(new LogHelper4TestsContext());
        }

        [TearDown]
        public void TestSettings()
        {
            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()
                       .StartFileBasedDi(new FileBasedConfigurationParameters(new FileBasedConfigurationFileContentsProvider(
                               Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration_Overview.xml")),
                           Helpers.TestsEntryAssemblyFolder, new LoadedAssembliesForTests())
                       {
                           AttributeValueTransformers = new[] { new FileFolderPathAttributeValueTransformer() }
                       }, out _)
                       .WithoutPresetDiContainer()
                       .AddAdditionalDiModules(new TestDiModule())
                       .RegisterModules()
                       .Start())
            {
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
