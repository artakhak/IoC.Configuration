using IoC.Configuration.Tests.ConstructedValue.Services;
using IoC.Configuration.Tests.TestTemplateFiles;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IoC.Configuration.Tests.ConstructedValue
{
    public abstract class ConstructedValueSuccessfulLoadTests : IoCConfigurationTestsForSuccessfulLoad
    {
        protected readonly static string ConstructedValueConfigurationRelativePath = "IoCConfiguration_constructedValue.xml";

        [Test]
        public void ConstructedValueAsParameterSerializerConstrutorParameterTests()
        {
            var appInfoSerializer = (AppInfoSerializer)Configuration.ParameterSerializers.TypeBasedSimpleSerializerAggregator.GetSerializerForType(typeof(IAppInfo));

            Assert.IsInstanceOf<AppDescriptionFormatter>(appInfoSerializer.AppDescriptionFormatter);

            AppDescriptionFormatter appDescriptionFormatter = (AppDescriptionFormatter)appInfoSerializer.AppDescriptionFormatter;

            Assert.AreEqual("AppData::", appDescriptionFormatter.PrefixToAddToDescription);
            Assert.AreEqual("::AppDataEnd", appDescriptionFormatter.PostfixToAddToDescription);

            var appInfo1InSettings = Settings.GetSettingValueOrThrow<IAppInfo>("App1");

            Assert.AreEqual(1, appInfo1InSettings.Id);
            Assert.AreEqual(appDescriptionFormatter.FormatDescription(appInfo1InSettings).Description, appInfo1InSettings.Description);
        }

        [Test]
        public void ConstructedValueAsSettingTests()
        {
            var appInfo2InSettings = Settings.GetSettingValueOrThrow<IAppInfo>("App2");

            Assert.AreEqual(2, appInfo2InSettings.Id);
            Assert.AreEqual("App 2", appInfo2InSettings.Description);
        }

        [Test]
        public void ConstructedValueInjectedIntoModuleConstructorParametersAndPropertiesTests()
        {
            var module1 = (Module1)Configuration.DependencyInjection.Modules.Modules.FirstOrDefault(x => x.DiModule is Module1).DiModule;

            Assert.AreEqual(3, module1.AppInfo.Id);
            Assert.AreEqual("App 3", module1.AppInfo.Description);
        }

        [Test]
        public void ConstructedValueInValueImplementationTests()
        {
            var appInfo = DiContainer.Resolve<IAppInfo>();
            Assert.AreEqual(8, appInfo.Id);
            Assert.AreEqual("App 8", appInfo.Description);
        }

        [Test]
        public void ConstructedValueInjectedIntoStartupActionTests()
        {
            var startupActionsRetriever = DiContainer.Resolve<StartupActionsRetriever>();

            var startupAction1 = (Services.StartupAction1)startupActionsRetriever.StartupActions[0];
            Assert.AreEqual(true, startupAction1.ActionExecutionCompleted);
            Assert.AreEqual(9, startupAction1.AppInfo.Id);
            Assert.AreEqual("App 9", startupAction1.AppInfo.Description);
        }

        [Test]
        public void ConstructedValueInCollectionTests()
        {
            var readonlyListOfAppInfo = DiContainer.Resolve<IReadOnlyList<IAppInfo>>();
            Assert.AreEqual(10, readonlyListOfAppInfo[0].Id);
            Assert.AreEqual("App 10", readonlyListOfAppInfo[0].Description);
        }

        [Test]
        public void ConstructedValueInAutoMethodAndAndAutoPropertyTests()
        {
            var appInfoFactory = DiContainer.Resolve<IAppInfoFactory>();

            Assert.AreEqual(11, appInfoFactory.DefaultAppInfo.Id);
            Assert.AreEqual("App 11", appInfoFactory.DefaultAppInfo.Description);

            var appInfo = appInfoFactory.CreateAppInfo();

            Assert.AreEqual(12, appInfo.Id);
            Assert.AreEqual("App 12", appInfo.Description);
        }

        [Test]
        public void ConstructedValueAsConstructorParameterInAnotherConstructedValueTests()
        { 
            var decoratedAppInfoSetting = Settings.GetSettingValueOrThrow<AppInfoDecorator>("DecoratedAppInfo");

            Assert.AreEqual(25, decoratedAppInfoSetting.Id);

            var appInfoDecorator = new AppInfoDecorator(new AppInfoDecorator(new AppInfo(25, "App 25")));
            Assert.AreEqual(appInfoDecorator.Description, $"App 25:{typeof(AppInfoDecorator).Name}:{typeof(AppInfoDecorator).Name}");
            Assert.AreEqual(appInfoDecorator.Description, decoratedAppInfoSetting.Description);
        }
    }
}