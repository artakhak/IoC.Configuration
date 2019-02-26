using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.Tests.ProxyService.Services;
using IoC.Configuration.Tests.TestTemplateFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ProxyService
{
    [TestClass]
    public class ProxyServiceFailedLoadTests : IoCConfigurationTestsBase
    {
        private const string Plugin1Name = "Plugin1";


        private void LoadConfigurationFile(DiImplementationType diImplementationType,
                                           Action<XmlDocument> modifyConfigurationFileOnLoad)
        {
            base.LoadConfigurationFile(diImplementationType, (container, configuration) => { }, null, modifyConfigurationFileOnLoad);
        }

        protected override string GetConfigurationRelativePath()
        {
            return "IoCConfiguration_proxyService.xml";
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void ProxyServiceIsSameTypeAsServiceToProxy(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/proxyService",
                                   (xmlElement) =>
                                   {
                                    return xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == "System.Collections.Generic.IEnumerable[System.Int32]";
                                   })
                               .SetAttributeValue(ConfigurationFileAttributeNames.Type, "System.Collections.Generic.List[System.Int32]");

                }), typeof(IServiceToProxyImplementationElement), typeof(ProxyServiceElement));
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void ProxyServiceIsNotAssignableFromServiceToProxy(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/proxyService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == typeof(IAppManager).FullName)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(IAppData).FullName);

                }), typeof(IServiceToProxyImplementationElement), typeof(ProxyServiceElement));
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void PluginProxyServiceInNonPluginSection(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/proxyService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == typeof(IAppManager).FullName)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Type, "TestPluginAssembly1.Interfaces.IDemoProxyService");

                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/proxyService/serviceToProxy",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == 
                                                 typeof(IAppManager_Extension).FullName)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Type, "TestPluginAssembly1.Interfaces.IDemoProxyService_Extension");

                }), typeof(ProxyServiceElement));
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void NonPluginProxyServiceInPluginSection(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/services/proxyService",
                               (element) =>
                               {
                                   return element.GetAttribute(ConfigurationFileAttributeNames.Type) == "TestPluginAssembly1.Interfaces.IDemoProxyService";
                               })
                               .SetAttributeValue(ConfigurationFileAttributeNames.Type, "IoC.Configuration.Tests.ProxyService.Services.IAppManager");

                    xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/services/proxyService/serviceToProxy",
                               (element) =>
                               {
                                   return element.GetAttribute(ConfigurationFileAttributeNames.Type) == "TestPluginAssembly1.Interfaces.IDemoProxyService_Extension";
                               })
                               .SetAttributeValue(ConfigurationFileAttributeNames.Type, "IoC.Configuration.Tests.ProxyService.Services.IAppManager_Extension");

                }), typeof(ProxyServiceElement));
        }
    }
}
