// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder.CodeBased;
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;
using NUnit.Framework;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using IoC.Configuration.DiContainerBuilder.FileBased;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.DiContainerBuilderTests
{
    [TestFixture]
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
            return (T)GlobalsCoreAmbientContext.Context.CreateInstance(assembly.GetType(classFullName), constructorParameters, out var errorMessage);
        }

        [TearDown]
        public void TestCleanup()
        {
            // Remove assembly resolution...
        }

        
        [TestCase(DiImplementationType.Autofac, true)]
        [TestCase(DiImplementationType.Autofac, false)]
        [TestCase(DiImplementationType.Ninject, true)]
        [TestCase(DiImplementationType.Ninject, false)]
        public void TestCodeBasedDiContainerBuilder(DiImplementationType diImplementationType, bool isLoggerNotSetTest)
        {
            var assemblyProbingPaths = new List<string>
            {
                DiManagerHelpers.ThirdPartyLibsFolder,
                DiManagerHelpers.DynamicallyLoadedDllsFolder,
                DiManagerHelpers.GetDiImplementationInfo(diImplementationType).DiManagerFolder
            };

            RuntTests(isLoggerNotSetTest, () =>
            {
                TestCodeBasedDiContainerBuilder(diImplementationType, assemblyProbingPaths, true);
                TestCodeBasedDiContainerBuilder(diImplementationType, assemblyProbingPaths, false);
            });
        }

        public void TestCodeBasedDiContainerBuilder(DiImplementationType diImplementationType,
                                                    IEnumerable<string> assemblyProbingPaths,
                                                    bool isPresetDiContainer)
        {
            var diImplementationInfo = DiManagerHelpers.GetDiImplementationInfo(diImplementationType);
            IDiContainer presetDiContainer = null;

            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();

            var diContainerConfigurator = diContainerBuilder.StartCodeBasedDi(diImplementationInfo.DiManagerClassName,
                diImplementationInfo.DiManagerAssemblyPath,
                new ParameterInfo[0], Helpers.TestsEntryAssemblyFolder,
                assemblyProbingPaths.ToArray());

            ICodeBasedDiModulesConfigurator diModulesConfigurator = null;
            if (isPresetDiContainer)
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
                if (isPresetDiContainer)
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

        
        [TestCase(DiImplementationType.Autofac, true)]
        [TestCase(DiImplementationType.Autofac, false)]
        [TestCase(DiImplementationType.Ninject, true)]
        [TestCase(DiImplementationType.Ninject, false)]
        public void TestFileBasedDiContainerBuilderWithDiContainerProvided(DiImplementationType diImplementationType, bool isLoggerNotSetTest)
        {
            RuntTests(isLoggerNotSetTest, () =>
            {
                var diContainer = CreateDiContainer(diImplementationType);

                var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();

                using (var containerInfo = diContainerBuilder.StartFileBasedDi(
                               new FileBasedConfigurationParameters(
                                   new FileBasedConfigurationFileContentsProvider(
                                       Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration_Overview.xml")),
                                   Helpers.TestsEntryAssemblyFolder,
                                   new LoadedAssembliesForTests())
                               {
                                   ConfigurationFileXmlDocumentLoaded = (sender, e) =>
                                   {
                                       Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"));
                                       ConfigurationFileXmlDocumentLoadedEventHandler(diImplementationType, e);
                                   },
                                   AttributeValueTransformers = new[] { new FileFolderPathAttributeValueTransformer() }
                               }, out _)
                           .WithDiContainer(diContainer)
                           .AddAdditionalDiModules(new DiModule1())
                           .AddNativeModules(CreateNativeModule(diImplementationType))
                           .RegisterModules()
                           .Start())
                {
                    Assert.AreSame(diContainer, containerInfo.DiContainer);

                    ValidateFileBasedDiContainer(diImplementationType, containerInfo.DiContainer);
                }
            });

            
        }

        
        [TestCase(DiImplementationType.Autofac, true)]
        [TestCase(DiImplementationType.Autofac, false)]
        [TestCase(DiImplementationType.Ninject, true)]
        [TestCase(DiImplementationType.Ninject, false)]
        public void TestFileBasedDiContainerBuilderWithNoDiContainerProvided(DiImplementationType diImplementationType, bool isLoggerNotSetTest)
        {
            RuntTests(isLoggerNotSetTest, () =>
            {
                var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();
                using (var containerInfo = diContainerBuilder.StartFileBasedDi(
                               new FileBasedConfigurationParameters(new FileBasedConfigurationFileContentsProvider(
                                       Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration_Overview.xml")), Helpers.TestsEntryAssemblyFolder,
                                   new LoadedAssembliesForTests())
                               {
                                   AttributeValueTransformers = new [] {new FileFolderPathAttributeValueTransformer()},
                                   ConfigurationFileXmlDocumentLoaded = (sender, e) =>
                                   {
                                       Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"));
                                       ConfigurationFileXmlDocumentLoadedEventHandler(diImplementationType, e);
                                   }
                               }, out _)
                               .WithoutPresetDiContainer()
                               .AddAdditionalDiModules(new DiModule1())
                               .AddNativeModules(CreateNativeModule(diImplementationType))
                               .RegisterModules()
                               .Start())
                {
                    ValidateFileBasedDiContainer(diImplementationType, containerInfo.DiContainer);
                }
            });
        }

        private void RuntTests(bool isLoggerNotSetTest, Action doRunTests)
        {
            try
            {
                LogHelper.RemoveContext();
                if (!isLoggerNotSetTest)
                    TestsHelper.SetupLogger();

                doRunTests();

                if (isLoggerNotSetTest)
                    Assert.Fail();
            }
            catch (LoggerWasNotInitializedException e)
            {
                if (!isLoggerNotSetTest)
                    Assert.Fail();

                Console.Out.WriteLine(e.Message);
            }
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