using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.Tests.TestTemplateFiles;
using NUnit.Framework;
using System;
using System.Xml;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ConstructedValue
{
    [TestFixture]
    public class ConstructedValueFailedLoadTests : IoCConfigurationTestsBase
    {
        private void LoadConfigurationFile(DiImplementationType diImplementationType,
                                           Action<XmlDocument> modifyConfigurationFileOnLoad)
        {
            base.LoadConfigurationFile(diImplementationType, (container, configuration) => { }, null, modifyConfigurationFileOnLoad);
        }

        protected override string GetConfigurationRelativePath()
        {
            return "IoCConfiguration_constructedValue.xml";
        }
        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidSettingReferenceInIfElement(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    var constructedValueParametersElement = xmlDocument.SelectElement("/iocConfiguration/settings/constructedValue/parameters");

                    constructedValueParametersElement.RemoveChildElement("int32");
                    constructedValueParametersElement.InsertChildElement(ConfigurationFileElementNames.ValueString)
                                                     .SetAttributeValue(ConfigurationFileAttributeNames.Name, "id")
                                                     .SetAttributeValue(ConfigurationFileAttributeNames.Value, "1");

                }), typeof(SettingElement));
        }
    }
}
