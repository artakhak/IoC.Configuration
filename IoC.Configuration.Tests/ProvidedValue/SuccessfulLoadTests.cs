using System;
using System.Collections.Generic;
using System.Linq;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.Tests.ProvidedValue.TestClasses;
using IoC.Configuration.Tests.TestTemplateFiles;
using NUnit.Framework;
using OROptimizer.Diagnostics.Log;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ProvidedValue;

public abstract class SuccessfulLoadTests : IoCConfigurationTestsForSuccessfulLoad
{
    private const string ConfigurationRelativePath = "IoCConfiguration_providedValue.xml";

    private static readonly ILog _logger = new LogToConsole();
    
    [Test]
    public void IoCModuleParameterResolved()
    {
    }
    
    protected static void OnClassInitialize(DiImplementationType diImplementationType)
    {
        OnClassInitialize(diImplementationType, ConfigurationRelativePath, null,
            null, new List<IValueProvider>
            {
                new ValueProvider(_logger)
            });
    }

    [Test]
    public void TestProvidedValueInjectionIntoModules()
    {
        // Logger is injected into constructor of Modules.Autofac.AutofacModule2 and
        // Modules.Ninject.NinjectModule2 and is registered in these modules.
        // Successfully resolving ILog means that the provided value was correctly injected into the module.
        var logger = DiContainer.Resolve<ILog>();
        Assert.That(logger, Is.SameAs(_logger));
        
        var dependencyInjectionElement = DiContainer.Resolve<IConfiguration>().DependencyInjection;
        
        Assert.That(Helpers.GetPropertyValue<int>(
            GetModule(dependencyInjectionElement, "Modules.IoC.DiModule3").DiModule, "DiModule3_Property1"), Is.EqualTo(37));
    }
    
    [Test]
    public void TestProvidedValueInjectionIntoPluginModules()
    {
        var dependencyInjectionElement = DiContainer.Resolve<IConfiguration>().PluginsSetup.AllPluginSetups.FirstOrDefault().DependencyInjection;

        if (DiImplementationType == DiImplementationType.Autofac)
            Assert.AreEqual(101, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "ModulesForPlugin1.Autofac.AutofacModule1").DiModule, "Property1"));
        else if (DiImplementationType == DiImplementationType.Ninject)
            Assert.AreEqual(101, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "ModulesForPlugin1.Ninject.NinjectModule1").DiModule, "Property1"));

        Assert.AreEqual(101, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "ModulesForPlugin1.IoC.DiModule1").DiModule, "Property1"));
    }
    
    private IModuleElement GetModule(IDependencyInjection dependencyInjection, string moduleType)
    {
        var allModules = dependencyInjection.Modules.Modules.Where(x => moduleType.Equals(x.DiModule.GetType().FullName, StringComparison.Ordinal)).ToList();

        Assert.AreEqual(1, allModules.Count);

        return allModules[0];
    }
    
    [Test]
    public void TestProvidedValueInjectionIntoSettings()
    {
        var logger = Settings.GetSettingValueOrThrow<ILog>("Logger");
        Assert.That(logger, Is.SameAs(_logger));
    }
    
    [Test]
    public void TestProvidedValueInjectionIntoPluginSettings()
    {
        var pluginRepository = DiContainer.Resolve<IPluginDataRepository>();

        var pluginData = pluginRepository.GetPluginData("Plugin1");
        
        Assert.AreEqual(pluginData.Settings.GetSettingValueOrThrow<int>("Int32Setting1"), 57);
        Assert.AreEqual(pluginData.Settings.GetSettingValueOrThrow<string>("StringSetting1"), "String Setting1 Value");
    }
    
    [Test]
    public void TestProvidedValueInjectionIntoServiceConstructorAndProperty()
    {
        var testInterface1 = DiContainer.Resolve<IoC.Configuration.Tests.ProvidedValue.TestClasses.ITestInterface1>();
        Assert.That(testInterface1.LoggerInjectedInConstructor, Is.SameAs(_logger));
        Assert.That(testInterface1.LoggerInjectedIntoProperty, Is.SameAs(_logger));
    }
    
    [Test]
    public void TestProvidedValueInjectionIntoPluginServiceConstructorAndProperty()
    {   
        var door = DiContainer.Resolve(Helpers.GetType("TestPluginAssembly1.Interfaces.IDoor"));
        var color = Helpers.GetPropertyValue<int>(door, "Color");
        var height = Helpers.GetPropertyValue<double>(door, "Height");
        
        Assert.That(color, Is.EqualTo(150));
        Assert.That(height, Is.EqualTo(90.1));
    }
    
    [Test]
    public void TestProvidedValueInjectedIntoPluginConstructorAndParameter()
    {
        var pluginRepository = DiContainer.Resolve<IPluginDataRepository>();

        var pluginData = pluginRepository.GetPluginData("Plugin1");
        
        Assert.AreEqual(Helpers.GetPropertyValue<long>(pluginData!.Plugin, "Property1"), (long)17, "Parameter injection via 'value provider' failed");
        Assert.AreEqual(Helpers.GetPropertyValue<long>(pluginData!.Plugin, "Property2"), (long)27, "Property injection via 'value provider' failed");
    }
}