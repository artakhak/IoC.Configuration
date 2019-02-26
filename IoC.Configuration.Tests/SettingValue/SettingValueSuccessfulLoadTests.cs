using IoC.Configuration.Tests.SettingValue.Services;
using IoC.Configuration.Tests.TestTemplateFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests.SettingValue
{
    public class SettingValueSuccessfulLoadTests : IoCConfigurationTestsForSuccessfullLoad
    {
        protected readonly static string SettingValueConfigurationRelativePath = "IoCConfiguration_settingValue_ReferencingInConfiguration.xml";


        [TestMethod]
        public void ValidateSettingValues()
        {
            Assert.AreEqual(-1, Settings.GetSettingValueOrThrow<int>("defaultInt"));
            Assert.AreEqual(7, Settings.GetSettingValueOrThrow<int>("defaultAppId"));
            Assert.AreEqual("Default App", Settings.GetSettingValueOrThrow<string>("defaultAppDescr"));
            Assert.AreEqual(37, Settings.GetSettingValueOrThrow<int>("app1"));
            Assert.AreEqual("Android", Settings.GetSettingValueOrThrow<string>("android"));
        }

        [TestMethod]
        public void SettingValueInValueImplementationTests()
        {
            int defaultInt = (int)DiContainer.Resolve(typeof(int));
            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("defaultInt"), defaultInt);
        }

        [TestMethod]
        public void SettingValueInCollectionTests()
        {
            var readonlyListOfInt = DiContainer.Resolve< System.Collections.Generic.IReadOnlyList<System.Int32> > ();

            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("defaultInt"), readonlyListOfInt[0]);
            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("app1"), readonlyListOfInt[1]);
        }

        [TestMethod]
        public void SettingValueInConstructedValueTests()
        {
            var appInfo = DiContainer.Resolve<IAppInfo>();
            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("defaultAppId"), appInfo.AppId);
            Assert.AreEqual(Settings.GetSettingValueOrThrow<string>("defaultAppDescr"), appInfo.AppDescription);
        }

        [TestMethod]
        public void SettingValueInAutoMethodIfConditionAndReturnValuesTests()
        {
            var appIds = DiContainer.Resolve<IAppIds>();

            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("defaultAppId"), appIds.GetDefaultAppId());

            var androidAppIds = appIds.GetAppIds(Settings.GetSettingValueOrThrow<string>("android"));

            Assert.AreEqual(3, androidAppIds.Count);
            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("defaultAppId"), androidAppIds[0]);
            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("app1"), androidAppIds[1]);

            var defaultPlatformAppIds = appIds.GetAppIds("something_else");
            Assert.AreEqual(2, defaultPlatformAppIds.Count);

            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("defaultAppId"), defaultPlatformAppIds[0]);
            Assert.AreEqual(8, defaultPlatformAppIds[1]);
        }

        [TestMethod]
        public void SettingValueInAutoPropertyReturnValuesTests()
        {
            var appIds = DiContainer.Resolve<IAppIds>();
            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("defaultAppId"), appIds.MainAppId);
        }

        [TestMethod]
        public void SettingValueInPlugin()
        {
            var doorType = Helpers.GetType("TestPluginAssembly1.Interfaces.IDoor");
            var doorObject = DiContainer.Resolve(doorType);

            var pluginSettings = DiContainer.Resolve<IPluginDataRepository>().GetPluginData("Plugin1").Settings;

            Assert.AreEqual("Brown", Settings.GetSettingValueOrThrow<string>("defaultColor"));
            Assert.AreEqual(4997399, pluginSettings.GetSettingValueOrThrow<int>("defaultColor"));

            Assert.AreEqual(pluginSettings.GetSettingValueOrThrow<int>("defaultColor"), (int)doorType.GetProperty("Color").GetValue(doorObject));
        }
    }
}