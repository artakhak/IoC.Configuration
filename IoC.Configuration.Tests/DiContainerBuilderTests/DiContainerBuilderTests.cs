using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder.CodeBased;
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using TestsSharedLibrary;

namespace IoC.Configuration.Tests.DiContainerBuilderTests
{
    [TestClass]
    public class DiContainerBuilderTests
    {
        #region Member Functions

        private void ConfigurationFileXmlDocumentLoadedEventHandler(DiImplementationType diImplementationType, [NotNull] ConfigurationFileXmlDocumentLoadedEventArgs e)
        {
            Helpers.ReplaceActiveDiManagerInConfigurationFile(e.XmlDocument, diImplementationType);

            var moduleClassNamesToRemove = new[] {"Modules.Autofac.AutofacModule1", "Modules.Ninject.NinjectModule1"};

            foreach (var moduleClassName in moduleClassNamesToRemove)
                e.XmlDocument.SelectElement("/iocConfiguration/dependencyInjection/modules/module",
                    x =>
                        moduleClassName.Equals(x.GetAttribute("type"), StringComparison.Ordinal)
                ).Remove();
        }

        private IDiContainer CreateDiContainer(DiImplementationType diImplementationType)
        {
            var diImplementationInfo = DiManagerHelpers.GetDiImplementationInfo(diImplementationType);

            var classFullName = diImplementationInfo.DiContainerClassName;
            var assemblyPath = diImplementationInfo.DiManagerAssemblyPath;
            var constructorParameters = new ParameterInfo[0];

            var probingPaths = new List<string>
            {
                DiManagerHelpers.ThirdPartyLibsFolder,
                diImplementationInfo.DiManagerFolder
            };

            using (new AssemblyResolver(probingPaths))
            {
                return CreateObject<IDiContainer>(classFullName, assemblyPath, constructorParameters);
            }
        }

        private object CreateNativeModule(DiImplementationType diImplementationType)
        {
            string classFullName = null;
            var assemblyPath = Path.Combine(DiManagerHelpers.DynamicallyLoadedDllsFolder, "TestProjects.Modules.dll");
            var constructorParameters = new ParameterInfo[0];

            var probingPaths = new List<string>
            {
                DiManagerHelpers.ThirdPartyLibsFolder,
                DiManagerHelpers.DynamicallyLoadedDllsFolder
            };

            switch (diImplementationType)
            {
                case DiImplementationType.Autofac:
                    classFullName = "Modules.Autofac.AutofacModule1";
                    constructorParameters = new[] {new ParameterInfo(typeof(int), 15)};
                    break;

                case DiImplementationType.Ninject:
                    classFullName = "Modules.Ninject.NinjectModule1";
                    constructorParameters = new[] {new ParameterInfo(typeof(int), 15)};
                    break;
            }

            using (new AssemblyResolver(probingPaths))
            {
                return CreateObject<object>(classFullName, assemblyPath, constructorParameters);
            }
        }

        private T CreateObject<T>(string classFullName, string assemblyPath, ParameterInfo[] constructorParameters) where T : class
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            GlobalsCoreAmbientContext.Context.TryCreateInstanceFromType(assembly.GetType(classFullName), constructorParameters, out var instance, out var errorMessage);
            return (T) instance;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Remove assembly resolution...
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TestCodeBasedDiContainerBuilder(DiImplementationType diImplementationType)
        {
            var assemblyProbingPaths = new List<string>
            {
                DiManagerHelpers.ThirdPartyLibsFolder,
                DiManagerHelpers.DynamicallyLoadedDllsFolder,
                DiManagerHelpers.GetDiImplementationInfo(diImplementationType).DiManagerFolder
            };

            TestCodeBasedDiContainerBuilder(diImplementationType, assemblyProbingPaths, true);
            TestCodeBasedDiContainerBuilder(diImplementationType, assemblyProbingPaths, false);
        }

        public void TestCodeBasedDiContainerBuilder(DiImplementationType diImplementationType,
                                                    IEnumerable<string> assemblyProbingPaths,
                                                    bool usPresetDiContainer)
        {
            var diImplementationInfo = DiManagerHelpers.GetDiImplementationInfo(diImplementationType);
            IDiContainer presetDiContainer = null;

            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();

            var diContainerConfigurator = diContainerBuilder.StartCodeBasedDi(diImplementationInfo.DiManagerClassName,
                diImplementationInfo.DiManagerAssemblyPath,
                new ParameterInfo[0], Helpers.TestsEntryAssemblyFolder,
                assemblyProbingPaths.ToArray());

            ICodeBasedDiModulesConfigurator diModulesConfigurator = null;
            if (usPresetDiContainer)
            {
                presetDiContainer = CreateDiContainer(diImplementationType);
                diModulesConfigurator = diContainerConfigurator.WithDiContainer(presetDiContainer);
            }
            else
            {
                diModulesConfigurator = diContainerConfigurator.WithoutPresetDiContainer();
            }

            string nativeModuleClassName = null;
            var nativeModuleClassAssemblyFilePath = Path.Combine(DiManagerHelpers.DynamicallyLoadedDllsFolder, "TestProjects.Modules.dll");

            switch (diImplementationType)
            {
                case DiImplementationType.Autofac:
                    nativeModuleClassName = "Modules.Autofac.AutofacModule1";
                    break;
                case DiImplementationType.Ninject:
                    nativeModuleClassName = "Modules.Ninject.NinjectModule1";
                    break;
            }

            using (var containerInfo = diModulesConfigurator
                                       .AddNativeModule(nativeModuleClassName, nativeModuleClassAssemblyFilePath,
                                           new[] {new ParameterInfo(typeof(int), 18)})
                                       .AddDiModules(new DiModule1())
                                       .RegisterModules()
                                       .Start())
            {
                if (usPresetDiContainer)
                    Assert.AreSame(presetDiContainer, containerInfo.DiContainer);

                ValidateDiContainer(diImplementationType, containerInfo.DiContainer);
            }
        }

        private void TestCodeBasedIoCBuilderWithNoDiContainerProvided(string diManagerClassFullName,
                                                                      string diManagerClassAssemblyFilePath,
                                                                      ParameterInfo[] diManagerConstructorParameters,
                                                                      params string[] assemblyProbingPaths)
        {
            var ioCBuilder = new DiContainerBuilder.DiContainerBuilder();
            ioCBuilder.StartCodeBasedDi(diManagerClassFullName, diManagerClassAssemblyFilePath, diManagerConstructorParameters, Helpers.TestsEntryAssemblyFolder, assemblyProbingPaths);
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TestFileBasedDiContainerBuilderWithDiContainerProvided(DiImplementationType diImplementationType)
        {
            var diContainer = CreateDiContainer(diImplementationType);

            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();

            using (var containerInfo = diContainerBuilder.StartFileBasedDi(
                                                             new FileBasedConfigurationFileContentsProvider(
                                                                 Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration1.xml")), Helpers.TestsEntryAssemblyFolder,
                                                             (sender, e) => ConfigurationFileXmlDocumentLoadedEventHandler(diImplementationType, e))
                                                         .WithDiContainer(diContainer)
                                                         .AddAdditionalDiModules(new DiModule1())
                                                         .AddNativeModules(CreateNativeModule(diImplementationType))
                                                         .RegisterModules()
                                                         .Start())
            {
                Assert.AreSame(diContainer, containerInfo.DiContainer);

                ValidateFileBasedDiContainer(diImplementationType, containerInfo.DiContainer);
            }
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TestFileBasedDiContainerBuilderWithNoDiContainerProvided(DiImplementationType diImplementationType)
        {
            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();
            using (var containerInfo = diContainerBuilder.StartFileBasedDi(
                                                             new FileBasedConfigurationFileContentsProvider(
                                                                 Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration1.xml")), Helpers.TestsEntryAssemblyFolder,
                                                             (sender, e) => ConfigurationFileXmlDocumentLoadedEventHandler(diImplementationType, e))
                                                         .WithoutPresetDiContainer()
                                                         .AddAdditionalDiModules(new DiModule1())
                                                         .AddNativeModules(CreateNativeModule(diImplementationType))
                                                         .RegisterModules()
                                                         .Start())
            {
                ValidateFileBasedDiContainer(diImplementationType, containerInfo.DiContainer);
            }
        }

        [TestInitialize]
        public void TestInit()
        {
            TestsHelper.SetupLogger();
        }

        private void ValidateDiContainer(DiImplementationType diImplementationType, IDiContainer diContainer)
        {
            // Validate resolutions in module DiModule1
            Assert.AreEqual(diContainer.Resolve<IService1>().GetType(), typeof(Service1));

            // Validate resolutions in native modules
            var interface1Test = diContainer.Resolve<ClassToTestServicesInjection<IInterface1>>();
            switch (diImplementationType)
            {
                case DiImplementationType.Autofac:
                    interface1Test.ValidateHasImplementation(typeof(Interface1_Impl1));
                    interface1Test.ValidateDoesNotHaveImplementation(typeof(Interface1_Impl2).FullName);
                    break;

                case DiImplementationType.Ninject:
                    interface1Test.ValidateHasImplementation(typeof(Interface1_Impl2));
                    interface1Test.ValidateDoesNotHaveImplementation(typeof(Interface1_Impl1).FullName);
                    break;
            }
        }


        private void ValidateFileBasedDiContainer(DiImplementationType diImplementationType, IDiContainer diContainer)
        {
            Assert.IsNotNull(diContainer.Resolve<ISettings>());
            Assert.IsNotNull(diContainer.Resolve<IPluginsSetup>());
            Assert.IsNotNull(diContainer.Resolve<IPluginDataRepository>());
            Assert.IsNotNull(diContainer.Resolve<IOnApplicationsStarted>());

            ValidateDiContainer(diImplementationType, diContainer);
        }

        #endregion

        #region Nested Types

        private class DiModule1 : ModuleAbstr
        {
            #region Member Functions

            protected override void AddServiceRegistrations()
            {
                Bind<IService1>().To<Service1>().SetResolutionScope(DiResolutionScope.Singleton);
                Bind<ClassToTestServicesInjection<IInterface1>>().ToSelf();
            }

            #endregion
        }

        private interface IService1
        {
        }

        private class Service1 : IService1
        {
        }

        #endregion
    }
}