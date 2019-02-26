using IoC.Configuration.Tests.TestTemplateFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using IoC.Configuration.ConfigurationFile;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ConstructedValue
{
    [TestClass]
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
       

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
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
