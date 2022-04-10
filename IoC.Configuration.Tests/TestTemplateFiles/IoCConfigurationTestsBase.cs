using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;
using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using TestsSharedLibrary;
using TestsSharedLibrary.DependencyInjection;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.TestTemplateFiles
{
    public abstract class IoCConfigurationTestsBase
    {
        [SetUp]
        public void TestInitialize()
        {
            TestsHelper.SetupLogger();
            Log4Tests.LogLevel = LogLevel.Debug;

            OnTestInitialize();
        }

        protected virtual void OnTestInitialize()
        {
           
        }

        [TearDown]
        public void TestCleanup()
        {
            OnTestCleanup();
            LogHelper.RemoveContext();

            OnTestCleanup();
        }

        protected virtual void OnTestCleanup()
        {

        }

        protected void LoadConfigurationFile(DiImplementationType diImplementationType, Action<IDiContainer, IConfiguration> onConfigurationLoaded,
                                             [CanBeNull] IDiModule[] additionalModulesToLoad = null,
                                             [CanBeNull] Action<XmlDocument> modifyConfigurationFileOnLoad = null)
        {
            var loadData = Helpers.LoadConfigurationFile(diImplementationType, GetConfigurationRelativePath(), additionalModulesToLoad, modifyConfigurationFileOnLoad);

            using (var containerInfo = loadData.containerInfo)
            {
                onConfigurationLoaded(containerInfo.DiContainer, containerInfo.DiContainer.Resolve<IConfiguration>());
            }
        }
      

        protected virtual string GetConfigurationRelativePath()
        {
            return Path.Combine("TestTemplateFiles", "IoCConfiguration_TestTemplate.xml");
        }
    }
}