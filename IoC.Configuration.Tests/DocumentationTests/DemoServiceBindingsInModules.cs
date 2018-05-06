using IoC.Configuration.Tests.SuccessfullDiModuleLoadTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer;
using System.IO;
using IoC.Configuration.DiContainer;
using IoC.Configuration.Tests.SuccessfullDiModuleLoadTests.TestClasses;
using TestsSharedLibrary;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.DocumentationTests
{
    [TestClass] 
    public class DemoServiceBindingsInModules
    {
        [TestInitialize]
        public void TestInitialize()
        {
            TestsHelper.SetupLogger();
        }

        [TestMethod]
        public void ResolveBindings()
        {
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
                var diContainer = containerInfo.DiContainer;

                SelfBoundServiceDemo(diContainer);
                BindToTypeDemo(diContainer);
                BindToAValueReturnedByDelegate(diContainer);
            }
        }

        private void SelfBoundServiceDemo(IoC.Configuration.DiContainer.IDiContainer diContainer)
        {
            var implementation = diContainer.Resolve<Class1>();
            Assert.IsTrue(implementation.GetType() == typeof(Class1));
        }

        private void BindToTypeDemo(IoC.Configuration.DiContainer.IDiContainer diContainer)
        {
            var implementation = diContainer.Resolve<IInterface2>();

            Assert.IsTrue(implementation.GetType() == typeof(Interface2_Impl1));

            // Validate that the implementation is an instance of the resolved type.
            Assert.IsInstanceOfType(implementation, typeof(IInterface2));
        }

        private void BindToAValueReturnedByDelegate(IoC.Configuration.DiContainer.IDiContainer diContainer)
        {
            var implementation = diContainer.Resolve<IInterface6>();
            Assert.IsInstanceOfType(implementation, typeof(IInterface6));
        }
    }
}
