using System;
using System.Xml;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.TestTemplateFiles
{
    /// <summary>
    /// Use this template to run tests that we want to load once on test initialize.
    /// </summary>
    /// <seealso cref="IoC.Configuration.Tests.TestTemplateFiles.IoCConfigurationTestsBase" />
    public abstract class IoCConfigurationTestsForSuccessfulLoad
    {
        protected static void OnClassInitialize(DiImplementationType diImplementationType,
                                                [NotNull] string configurationRelativePath,
                                                [CanBeNull] IDiModule[] additionalModulesToLoad = null,
                                                [CanBeNull] Action<XmlDocument> modifyConfigurationFileOnLoad = null)
        {
            var loadData = Helpers.LoadConfigurationFile(diImplementationType, configurationRelativePath,
                additionalModulesToLoad, modifyConfigurationFileOnLoad);

            ContainerInfo = loadData.containerInfo;
            DiContainer = ContainerInfo.DiContainer;
            Configuration = DiContainer.Resolve<IConfiguration>();
            Settings = DiContainer.Resolve<ISettings>();
            DiImplementationType = diImplementationType;
        }

        protected static void OnClassCleanup()
        {
            DiContainer?.Dispose();
            LogHelper.RemoveContext();
        }

        protected static IContainerInfo ContainerInfo { get; private set; }
        protected static IDiContainer DiContainer { get; private set; }
        protected static IConfiguration Configuration { get; private set; }
        protected static ISettings Settings { get; private set; }

        protected static DiImplementationType DiImplementationType { get; private set; }
    }
}