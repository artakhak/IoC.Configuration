using System;
using System.Collections.Generic;
using System.Xml;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.Tests.ProvidedValue.TestClasses;
using IoC.Configuration.Tests.TestTemplateFiles;
using NUnit.Framework;
using OROptimizer.Diagnostics.Log;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ProvidedValue;

[TestFixture]
public class FailedLoadTests : IoCConfigurationTestsBase
{
    private static readonly ILog _logger = new LogToConsole();
        
    private void LoadConfigurationFile(DiImplementationType diImplementationType,
        Action<XmlDocument> modifyConfigurationFileOnLoad,
        IReadOnlyList<IValueProvider> errorValueProviders)
    {
        base.LoadConfigurationFile(
            diImplementationType, (container, configuration) => { }, null, 
            modifyConfigurationFileOnLoad,
            errorValueProviders);
    }

    protected override string GetConfigurationRelativePath()
    {
        return "IoCConfiguration_providedValue.xml";
    }
        
    [TestCase(DiImplementationType.Autofac)]
    [TestCase(DiImplementationType.Ninject)]
    public void ProvidedValueIsOfInvalidType(DiImplementationType diImplementationType)
    {
        Helpers.TestExpectedConfigurationParseException(() =>
            LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                },
                new List<IValueProvider>()
                {
                    new TestValueProvider((IProvidedValueData providedValueData, out object resolvedValue) =>
                    {
                        if (providedValueData.ProvidedValueTargetType == ProvidedValueTargetType.ConstructorParameter &&
                            providedValueData.Name == "logger")
                        {
                            resolvedValue = 15;
                            return true;
                        }
                        resolvedValue = null;
                        return false;
                    }),
                    new ValueProvider(_logger)
                }
            ), typeof(ParameterElement));
    }
        
    [TestCase(DiImplementationType.Autofac)]
    [TestCase(DiImplementationType.Ninject)]
    public void NoValueIsProvided(DiImplementationType diImplementationType)
    {
        Helpers.TestExpectedConfigurationParseException(() =>
            LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {},
                Array.Empty<IValueProvider>()
            ), typeof(ParameterElement));
    }
}