
using IoC.Configuration.Tests.SuccessfulDiModuleLoadTests;
using IoC.Configuration.Tests.SuccessfulDiModuleLoadTests.TestClasses;
using NUnit.Framework;
using OROptimizer;
using System.IO;
using IoC.Configuration.DiContainerBuilder.FileBased;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary.DependencyInjection;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests
{
    [TestFixture]
    public class GitHubWikiDemoCodeTests
    {
        [TearDown]
        public void TestCleanup()
        {
            OROptimizer.Diagnostics.Log.LogHelper.RemoveContext();
        }

        [Test]
        public void FileConfigurationExample1()
        {
            TestsSharedLibrary.TestsHelper.SetupLogger();

            var configurationFileContentsProvider = new FileBasedConfigurationFileContentsProvider(
                Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration_Overview.xml"));

            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()
                       .StartFileBasedDi(
                           new FileBasedConfigurationParameters(configurationFileContentsProvider,
                               Helpers.TestsEntryAssemblyFolder,
                               new LoadedAssembliesForTests())
                           {
                               AdditionalReferencedAssemblies = new []
                               {
                                   // List additional assemblies that should be added to dynamically generated assembly as references
                                   Path.Combine(Helpers.GetTestFilesFolderPath(), @"DynamicallyLoadedDlls\TestProjects.DynamicallyLoadedAssembly1.dll"),
                                   Path.Combine(Helpers.GetTestFilesFolderPath(), @"DynamicallyLoadedDlls\TestProjects.DynamicallyLoadedAssembly2.dll")
                               },
                               AttributeValueTransformers = new[] {new FileFolderPathAttributeValueTransformer()},
                               ConfigurationFileXmlDocumentLoaded = (sender, e) =>
                               {
                                   // Replace some elements in e.XmlDocument if needed,
                                   // before the configuration is loaded.
                                   Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"));
                               }
                           }, out _)
                       .WithoutPresetDiContainer()
                       .AddAdditionalDiModules(new TestDiModule())
                       .RegisterModules()
                       .Start())
            {
                var container = containerInfo.DiContainer;

                Assert.IsNotNull(containerInfo.DiContainer.Resolve<IInterface6>());

                var settings = container.Resolve<ISettings>();
                Assert.AreEqual(155.7, settings.GetSettingValueOrThrow<double>("MaxCharge"));

                var pluginRepository = container.Resolve<IPluginDataRepository>();

                var pluginData = pluginRepository.GetPluginData("Plugin1");
                Assert.AreEqual(38, pluginData.Settings.GetSettingValueOrThrow<long>("Int64Setting1"));
            }
        }

        [Test]
        public void CodeBasedConfigurationExample1()
        {
            TestsSharedLibrary.TestsHelper.SetupLogger();

            // Probing paths are used to re-solve the dependencies.
            var assemblyProbingPaths = new string[]
            {
                DiManagerHelpers.ThirdPartyLibsFolder,
                DiManagerHelpers.DynamicallyLoadedDllsFolder,
                DiManagerHelpers.GetDiImplementationInfo(DiImplementationType.Autofac).DiManagerFolder
            };

            var diImplementationInfo = DiManagerHelpers.GetDiImplementationInfo(DiImplementationType.Autofac);

            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()
                .StartCodeBasedDi("IoC.Configuration.Autofac.AutofacDiManager",
                                  diImplementationInfo.DiManagerAssemblyPath,
                                  new ParameterInfo[0],
                                  Helpers.TestsEntryAssemblyFolder,
                                  assemblyProbingPaths)
                .WithoutPresetDiContainer()
                // Note, AddNativeModule() to add native modules (e.g., instances of  Autofac.AutofacModule or
                // Ninject.Modules.NinjectModule) // and AddDiModules to add IoC.Configuration modules (i.e.,
                // instances IoC.Configuration.DiContainer.IDiModule), can be called multiple times, without
                // any restriction on the order in which these methods are called.           
                .AddNativeModule("Modules.Autofac.AutofacModule1",
                                 Path.Combine(DiManagerHelpers.DynamicallyLoadedDllsFolder, "TestProjects.Modules.dll"), 
                                 new[] { new ParameterInfo(typeof(int), 18) })
                .AddDiModules(new TestDiModule())
                .RegisterModules()
                .Start())
            {
                var container = containerInfo.DiContainer;

                Assert.IsNotNull(containerInfo.DiContainer.Resolve<IInterface6>());
            }
        }
    }
}
