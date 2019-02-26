using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.Tests.TestTemplateFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Xml;
using IoC.Configuration.Tests.AutoService.Services;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.Collection
{
    [TestClass]
    public class CollectionFailedLoadTests : IoCConfigurationTestsBase
    {

        private void LoadConfigurationFile(DiImplementationType diImplementationType,
                                           Action<XmlDocument> modifyConfigurationFileOnLoad)
        {
            base.LoadConfigurationFile(diImplementationType, (container, configuration) => { }, null, modifyConfigurationFileOnLoad);
        }

        protected override string GetConfigurationRelativePath()
        {
            return "IoCConfiguration_collection.xml";
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void CollectionItemIsOfInvalidType(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>
                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/settings/constructedValue/parameters/collection",
                                   (xmlElement) =>
                                   {
                                       return xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "intValues";
                                   })
                               .InsertChildElement(ConfigurationFileElementNames.ValueString)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Value, "some text");
                }), typeof(CollectionItemValueElement), typeof(ParameterElement));
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void CollectionTypeIsInvalidForParameter(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>
                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/settings/constructedValue/parameters/collection",
                                   (xmlElement) =>
                                   {
                                       return xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "intValues";
                                   })
                               .SetAttributeValue(ConfigurationFileAttributeNames.CollectionType, "enumerable");
                }), typeof(SettingElement));
        }
    }
}
