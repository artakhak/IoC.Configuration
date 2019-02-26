using IoC.Configuration.Tests.TestTemplateFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using IoC.Configuration.ConfigurationFile;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ValueImplementation
{
    [TestClass]
    public class ValueImplementationFailedLoadTests : IoCConfigurationTestsBase
    {
        private const string Plugin1Name = "Plugin1";


        private void LoadConfigurationFile(DiImplementationType diImplementationType,
                                           Action<XmlDocument> modifyConfigurationFileOnLoad)
        {
            base.LoadConfigurationFile(diImplementationType, (container, configuration) => { }, null, modifyConfigurationFileOnLoad);
        }

        protected override string GetConfigurationRelativePath()
        {
            return "IoCConfiguration_valueImplementation.xml";
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void InvalidImplementationType(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>
                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/service/valueImplementation/settingValue",
                                   (xmlElement) =>
                                   {
                                       return xmlElement.GetAttribute(ConfigurationFileAttributeNames.SettingName) == "defaultAppId";
                                   })
                               .SetAttributeValue(ConfigurationFileAttributeNames.SettingName, "defaultAppDescription");

                }), typeof(IValueBasedServiceImplementationElement));
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void NonExistentSettingValueAsImplementation(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>
                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/service/valueImplementation/settingValue",
                                   (xmlElement) =>
                                   {
                                       return xmlElement.GetAttribute(ConfigurationFileAttributeNames.SettingName) == "defaultAppId";
                                   })
                               .SetAttributeValue(ConfigurationFileAttributeNames.SettingName, "defaultAppId_Invalid");

                }), typeof(SettingValueElement));
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void PluginServiceUsedInNonPluginSection(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>
                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {

                   var pluginServiceElement = xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/services/service",
                                   (xmlElement) =>
                                   {
                                       return xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == "System.Collections.Generic.IReadOnlyList[TestPluginAssembly1.Interfaces.IDoor]";
                                   });

                    pluginServiceElement.ParentNode.RemoveChild(pluginServiceElement);

                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services").AppendChild(pluginServiceElement);

                }), typeof(ServiceElement));
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void NonPluginServiceUsedInPluginSection(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>
                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {

                    var nonPluginServiceElement = xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/service",
                        (xmlElement) =>
                        {
                            return xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == "System.Collections.Generic.IReadOnlyList[IoC.Configuration.Tests.ValueImplementation.Services.IAppInfo]";
                        });

                    nonPluginServiceElement.ParentNode.RemoveChild(nonPluginServiceElement);

                    xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/services").AppendChild(nonPluginServiceElement);

                }), typeof(ServiceElement));
        }

    }
}
