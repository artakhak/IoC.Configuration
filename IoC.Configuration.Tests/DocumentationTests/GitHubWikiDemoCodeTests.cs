
using IoC.Configuration.Tests.SuccessfullDiModuleLoadTests;
using IoC.Configuration.Tests.SuccessfullDiModuleLoadTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer;
using System.IO;
using TestsSharedLibrary.DependencyInjection;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests
{
    [TestClass]
    public class GitHubWikiDemoCodeTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            OROptimizer.Diagnostics.Log.LogHelper.RemoveContext();
        }

        [TestMethod]
        public void FileConfigurationExample1()
        {
            OROptimizer.Diagnostics.Log.LogHelper.RegisterContext(new LogHelper4TestsContext());
            TestsSharedLibrary.TestsHelper.SetupLogger();

            var configurationFileContentsProvider = new FileBasedConfigurationFileContentsProvider(
                Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration1.xml"));

            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()
                           .StartFileBasedDi(configurationFileContentsProvider,
                                Helpers.TestsEntryAssemblyFolder,
                                (sender, e) =>
                                {
                                    // Replace some elements in e.XmlDocument if needed,
                                    // before the configuration is loaded.
                                })
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

        [TestMethod]
        public void CodeBasedConfigurationExample1()
        {
            OROptimizer.Diagnostics.Log.LogHelper.RegisterContext(new LogHelper4TestsContext());
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
