using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer.Diagnostics.Log;
using System;
using System.IO;
using System.Xml;
using TestsSharedLibrary;
using TestsSharedLibrary.DependencyInjection;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.TestTemplateFiles
{
    //public abstract class IoCConfigurationTestsForFailedLoad : IoCConfigurationTestsBase
    //{
    //    protected override void OnTestInitialize()
    //    {
    //        base.OnTestInitialize();

            
    //    }

    //    protected override void OnTestCleanup()
    //    {
    //        base.OnTestCleanup();
    //    }

    //    protected abstract DiImplementationType DiImplementationType { get; }
    //}
    public abstract class IoCConfigurationTestsBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            TestsHelper.SetupLogger();
            Log4Tests.LogLevel = LogLevel.Debug;

            OnTestInitialize();
        }

        protected virtual void OnTestInitialize()
        {
           
        }


        [TestCleanup]
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
