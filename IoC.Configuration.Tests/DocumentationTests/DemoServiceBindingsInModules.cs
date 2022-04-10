using IoC.Configuration.Tests.SuccessfulDiModuleLoadTests;
using IoC.Configuration.Tests.SuccessfulDiModuleLoadTests.TestClasses;
using NUnit.Framework;
using OROptimizer;
using System.IO;
using TestsSharedLibrary;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.DocumentationTests
{
    [TestFixture] 
    public class DemoServiceBindingsInModules
    {
        [SetUp]
        public void TestInitialize()
        {
            TestsHelper.SetupLogger();
        }

        [Test]
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
            Assert.IsInstanceOf<IInterface2>(implementation);
        }

        private void BindToAValueReturnedByDelegate(IoC.Configuration.DiContainer.IDiContainer diContainer)
        {
            var implementation = diContainer.Resolve<IInterface6>();
            Assert.IsInstanceOf<IInterface6>(implementation);
        }
    }
}
