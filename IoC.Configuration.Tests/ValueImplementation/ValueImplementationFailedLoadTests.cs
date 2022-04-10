using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.Tests.TestTemplateFiles;
using NUnit.Framework;
using System;
using System.Xml;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ValueImplementation
{
    [TestFixture]
    public class ValueImplementationFailedLoadTests : IoCConfigurationTestsBase
    {
        private void LoadConfigurationFile(DiImplementationType diImplementationType,
                                           Action<XmlDocument> modifyConfigurationFileOnLoad)
        {
            base.LoadConfigurationFile(diImplementationType, (_, _) => { }, null, modifyConfigurationFileOnLoad);
        }

        protected override string GetConfigurationRelativePath()
        {
            return "IoCConfiguration_valueImplementation.xml";
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
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

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
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

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
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

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
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
