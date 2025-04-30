// Copyright (c) IoC.Configuration Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.

using System;
using IoC.Configuration.Tests.SuccessfulDiModuleLoadTests;
using IoC.Configuration.Tests.SuccessfulDiModuleLoadTests.TestClasses;
using NUnit.Framework;
using OROptimizer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using IoC.Configuration.AttributeValueTransformer;
using IoC.Configuration.DiContainerBuilder.FileBased;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary.DependencyInjection;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.DocumentationTests
{
    [TestFixture]
    public class DemoLoadingIoCConfiguration
    {
        [SetUp]
        public void TestInitialize()
        {
            // Before the configuration is loaded, an instance of OROptimizer.Diagnostics.Log.ILogHelperContext
            // should be registered, using OROptimizer.Diagnostics.Log.LogHelper.RegisterContext(ILogHelperContext).
            // An implementation of ILogHelperContext for log4net,
            // OROptimizer.Log4Net.Log4NetHelperContext, can be found in Nuget package OROptimizer.Shared.Log4Net.
            if (!OROptimizer.Diagnostics.Log.LogHelper.IsContextInitialized)
                OROptimizer.Diagnostics.Log.LogHelper.RegisterContext(new LogHelper4TestsContext());
        }

        [Test]
        public void LoadFromXmlConfigurationFile()
        {
            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()
                       .StartFileBasedDi(
                           new FileBasedConfigurationParameters(
                               new FileBasedConfigurationFileContentsProvider(
                                   Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration_Overview.xml")),
                               // Provide the entry assembly folder. Normally this is the folder,
                               // where the executable file is. However for test projects this might not
                               // be the case. This folder will be used in assembly resolution.
                               Helpers.TestsEntryAssemblyFolder, new LoadedAssembliesForTests())
                           {
                               AdditionalReferencedAssemblies = new string[]
                               {
                                   // List additional assemblies that should be added to dynamically generated assembly as references
                                   Path.Combine(Helpers.GetTestFilesFolderPath(), @"DynamicallyLoadedDlls\TestProjects.DynamicallyLoadedAssembly1.dll"),
                                   Path.Combine(Helpers.GetTestFilesFolderPath(), @"DynamicallyLoadedDlls\TestProjects.DynamicallyLoadedAssembly2.dll")
                               },
                               // Set the value of AttributeValueTransformers to list of 
                               // IoC.Configuration.AttributeValuesProvider.IAttributeValueTransformer instances
                               // to change some xml attribute values when the xml configuration is loaded,
                               // before the configuration is parsed.
                               // Good example of implementation of IoC.Configuration.AttributeValuesProvider.IAttributeValueTransformer
                               // is IoC.Configuration.Tests.FileFolderPathAttributeValueTransformer.
                               AttributeValueTransformers = new IAttributeValueTransformer []
                               {
                                   new FileFolderPathAttributeValueTransformer()
                               },
                               ConfigurationFileXmlDocumentLoaded = (sender, e) =>
                               {
                                   // Replace some elements in e.XmlDocument if needed,
                                   // before the configuration is loaded.
                                   // For example, we can replace the value of attribute 'activeDiManagerName' in element 
                                   // iocConfiguration.diManagers to use a different DI manager (say
                                   // switch from Autofac to Ninject).
                                   Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"));
                                   e.XmlDocument.SelectElements("/iocConfiguration/diManagers")
                                       .First()
                                       .SetAttributeValue("activeDiManagerName", "Autofac");
                               }
                           }, out _)

                       // Note, most of the time we will need to call method WithoutPresetDiContainer().
                       // However, in some cases, we might need to create an instance of IoC.Configuration.DiContainer.IDiContainer,
                       // and call the method WithDiContainer(IoC.Configuration.DiContainer.IDiContainer diContainer) instead.
                       // This might be necessary when using the IoC.Configuration to configure dependency injection in 
                       // ASP.NET Core projects.
                       // An example implementation of IDIContainer is IoC.Configuration.Autofac.AutofacDiContainer in 
                       // Nuget package IoC.Configuration.Autofac.
                       .WithoutPresetDiContainer()

                       // Note, native and IoC.Configuration modules can be specified in XML configuration file, in
                       // iocConfiguration/dependencyInjection/modules/module elements.
                       // However, if necessary, AddAdditionalDiModules() and AddNativeModules() can be used to load additional
                       // IoC.Configuration modules (instances of IoC.Configuration.DiContainer.IDiModule), as well
                       // as native (e.g, Ninject or Autofac) modules.
                       // Also, AddAdditionalDiModules() and AddNativeModules() can be called multiple times in any order.
                       .AddAdditionalDiModules(new TestDiModule())
                       .AddNativeModules(CreateModule<object>("Modules.Autofac.AutofacModule1", 
                           new ParameterInfo[] { new ParameterInfo(typeof(int), 5) }))
                       .RegisterModules()
                       .Start())
            {
                var diContainer = containerInfo.DiContainer;

                // Once the configuration is loaded, resolve types using IoC.Configuration.DiContainer.IDiContainer
                // Note, interface IoC.Configuration.DiContainerBuilder.IContainerInfo extends System.IDisposable,
                // and should be disposed, to make sure all the resources are properly disposed of.
                var resolvedInstance = containerInfo.DiContainer.Resolve<SharedServices.Interfaces.IInterface7>();
            }
        }

        [Test]
        public void LoadFromModules()
        {
            var diImplementationInfo = DiManagerHelpers.GetDiImplementationInfo(DiImplementationType.Autofac);

            var assemblyProbingPaths = new[]
            {
                DiManagerHelpers.ThirdPartyLibsFolder,
                diImplementationInfo.DiManagerFolder
            };
           
            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()

                    // Class IoC.Configuration.DiContainerBuilder.DiContainerBuilder has two overloaded methods StartCodeBasedDi(...)
                    // DiContainerBuilder.StartCodeBasedDi(IoC.Configuration.DiContainer.IDiManager diManager,...) and
                    // DiContainerBuilder.StartCodeBasedDi(string diManagerClassFullName, string diManagerClassAssemblyFilePath,...).
                    // if the project references the library with implementation of IoC.Configuration.DiContainer.IDiManager,
                    // the first one can be used. Otherwise the second overloaded method can be used, in which case reflection will be used to
                    // create an instance of IoC.Configuration.DiContainer.IDiManager.
                    .StartCodeBasedDi("IoC.Configuration.Autofac.AutofacDiManager",
                                   Path.Combine( Helpers.GetTestFilesFolderPath(),
                                       @"ContainerImplementations\Autofac\IoC.Configuration.Autofac.dll"),
                                   Array.Empty<ParameterInfo>(), Helpers.TestsEntryAssemblyFolder, assemblyProbingPaths)
                    // Note, most of the time we will need to call method WithoutPresetDiContainer().
                    // However, in some cases, we might need to create an instance of IoC.Configuration.DiContainer.IDiContainer,
                    // and call the method WithDiContainer(IoC.Configuration.DiContainer.IDiContainer diContainer) instead.
                    // This might be necessary when using the IoC.Configuration to configure dependency injection in 
                    // ASP.NET Core projects.
                    // An example implementation of IDIContainer is IoC.Configuration.Autofac.AutofacDiContainer in 
                    // Nuget package IoC.Configuration.Autofac.
                    .WithoutPresetDiContainer()

                    // The methods AddDiModules(params IDiModule[] diModules),
                    // AddNativeModules(params object[] nativeModules), and
                    // AddNativeModules(string nativeModuleClassFullName, string nativeModuleClassAssemblyFilePath, ...)
                    // are used to load IoC.Configuration modules (instances of IoC.Configuration.DiContainer.IDiModule), as well
                    // as native (e.g, Ninject or Autofac) modules.
                    // Also, these three methods can be called multiple times in any order.
                    .AddDiModules(new TestDiModule())
                    .AddNativeModule("Modules.Autofac.AutofacModule1",
                        Path.Combine(Helpers.GetTestFilesFolderPath(), 
                        @"DynamicallyLoadedDlls\TestProjects.Modules.dll"),
                        new ParameterInfo[] { new ParameterInfo(typeof(int), 5) })
                                       
                    .RegisterModules()
                    .Start())
            {
                var diContainer = containerInfo.DiContainer;

                // Once the configuration is loaded, resolve types using IoC.Configuration.DiContainer.IDiContainer
                // Note, interface IoC.Configuration.DiContainerBuilder.IContainerInfo extends System.IDisposable,
                // and should be disposed, to make sure all the resources are properly disposed of.
                var resolvedInstance = containerInfo.DiContainer.Resolve<IInterface2>();
            }
        }

        private static T CreateModule<T>(string moduleTypeFullName, ParameterInfo[] constructorParameters) where T : class
        {
            var assemblyPath = Path.Combine(DiManagerHelpers.DynamicallyLoadedDllsFolder, "TestProjects.Modules.dll");

            return CreateObject<T>(moduleTypeFullName, assemblyPath, constructorParameters);
        }

        private static T CreateObject<T>(string classFullName, string assemblyPath, ParameterInfo[] constructorParameters) where T : class
        {
            var probingPaths = new List<string>
            {
                DiManagerHelpers.ThirdPartyLibsFolder,
                DiManagerHelpers.DynamicallyLoadedDllsFolder
            };

            using (new AssemblyResolver(probingPaths))
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                return (T)GlobalsCoreAmbientContext.Context.CreateInstance(assembly.GetType(classFullName), constructorParameters, out var errorMessage);
            }
        }
    }
    
}
