using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using SharedServices.Interfaces;
using TestsSharedLibrary;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Configuration.Tests.DocumentationTests
{
    [TestClass] 
    public class DemoTypeResolutions
    {
        [TestInitialize]
        public void TestInitialize()
        {
            TestsHelper.SetupLogger();
        }


        [TestMethod]
        public void ResolveBindings()
        {
            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();
            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder().StartFileBasedDi(
                                                   new FileBasedConfigurationFileContentsProvider(
                                                       Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration1.xml")),
                                                   Helpers.TestsEntryAssemblyFolder,
                                                   (sender, e) =>
                                                   {
                                                       // Replace some elements in e.XmlDocument if needed,
                                                       // before the configuration is loaded.
                                                   })
                                               .WithoutPresetDiContainer()
                                              
                                               .RegisterModules()
                                               .Start())
            {
                var diContainer = containerInfo.DiContainer;

                LifetimeScopeResolutionExample(diContainer);
                SingletonScopeResolutionExample(diContainer);
                TransientScopeResolutionExample(diContainer);

                ResolvingATypeWithMultipleBindings(diContainer);
            }
        }

        private void SingletonScopeResolutionExample(IoC.Configuration.DiContainer.IDiContainer diContainer)
        {
            var service1 = diContainer.Resolve<IInterface9>();
            var service2 = diContainer.Resolve<IInterface9>();
            Assert.AreSame(service1, service2);
        }

        private void TransientScopeResolutionExample(IoC.Configuration.DiContainer.IDiContainer diContainer)
        {
            Type typeInterface2 = Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface2");
            
            var service1 = diContainer.Resolve(typeInterface2);
            var service2 = diContainer.Resolve(typeInterface2);
            Assert.AreNotSame(service1, service2);
        }
        private void LifetimeScopeResolutionExample(IoC.Configuration.DiContainer.IDiContainer diContainer)
        {
            Type typeInterface3 = Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface3");

            // Same objects are created in default lifetime scope.
            var service1InMainScope = diContainer.Resolve(typeInterface3);
            var service2InMainScope = diContainer.Resolve(typeInterface3);

            Assert.AreSame(service1InMainScope, service2InMainScope);

            using (var lifeTimeScope = diContainer.StartLifeTimeScope())
            {
                // IDiContainer.Resolve(Type, ILifetimeScope) returns the same object for the same scope lifeTimeScope.
                var service1InScope1 = diContainer.Resolve(typeInterface3, lifeTimeScope);
                var service2InScope1 = diContainer.Resolve(typeInterface3, lifeTimeScope);

                Assert.AreSame(service1InScope1, service2InScope1);

                // However, the object are different from the ones created in main lifetime scope.
                Assert.AreNotSame(service1InScope1, service1InMainScope);
            }
        }

        private void ResolvingATypeWithMultipleBindings(IoC.Configuration.DiContainer.IDiContainer diContainer)
        {
            var resolvedInstances = diContainer.Resolve<IEnumerable<SharedServices.Interfaces.IInterface5>>()
                                               .ToList();

            Assert.AreEqual(3, resolvedInstances.Count);

            var typeOfInterface5 = typeof(IInterface5);
            Assert.IsInstanceOfType(resolvedInstances[0], typeOfInterface5);
            Assert.IsInstanceOfType(resolvedInstances[1], typeOfInterface5);
            Assert.IsInstanceOfType(resolvedInstances[2], typeOfInterface5);
        }
    }
}
