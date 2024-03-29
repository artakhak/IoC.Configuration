﻿using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.Tests.AutoService.Services;
using IoC.Configuration.Tests.TestTemplateFiles;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Xml;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.AutoService
{
    [TestFixture]
    public class AutoServiceFailedLoadTests : IoCConfigurationTestsBase
    {
        private void LoadConfigurationFile(DiImplementationType diImplementationType,
                                           Action<XmlDocument> modifyConfigurationFileOnLoad)
        {
            base.LoadConfigurationFile(diImplementationType, (container, configuration) => { }, null, modifyConfigurationFileOnLoad);
        }

        protected override string GetConfigurationRelativePath()
        {
            return "IoCConfiguration_autoService.xml";
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void NonExistentServiceTypeReference(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                            xmlElement => xmlElement.GetAttribute("interfaceRef") == "IProjectGuids")
                                   .SetAttribute("interfaceRef", "IProjectGuidsInvalid");
                }), typeof(IAutoGeneratedServiceElement));
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void PluginServiceInNonPluginSection(DiImplementationType diImplementationType)
        {
            // Lets first add this service in plugin section to make sure it works.
            LoadConfigurationFile(diImplementationType, (xmlDocument) =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/autoGeneratedServices")
                           .InsertChildElement(ConfigurationFileElementNames.AutoService)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Interface, "TestPluginAssembly1.Interfaces.IRoom");
            });

            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices")
                               .InsertChildElement(ConfigurationFileElementNames.AutoService)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Interface, "TestPluginAssembly1.Interfaces.IRoom");
                }), typeof(IAutoGeneratedServiceElement));
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void NonPluginServiceInPluginSection(DiImplementationType diImplementationType)
        {
            // Lets first add this service in non-plugin section to make sure it works.
            LoadConfigurationFile(diImplementationType, (xmlDocument) =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices")
                           .InsertChildElement(ConfigurationFileElementNames.AutoService)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Interface, typeof(SharedServices.Interfaces.IInterface12).FullName);
            });

            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/autoGeneratedServices")
                               .InsertChildElement(ConfigurationFileElementNames.AutoService)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Interface, typeof(SharedServices.Interfaces.IInterface12).FullName);
                }), typeof(IAutoGeneratedServiceElement));
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void NonExistentServiceType(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                        xmlElement => xmlElement.GetAttribute("interfaceRef") == "IProjectGuids")
                               .Remove("interfaceRef")
                               .SetAttribute("interface", "IProjectGuidsInvalid");

                }), typeof(IAutoGeneratedServiceElement));
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void NonInterfaceServiceType(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute("interfaceRef") == "IProjectGuids")
                               .Remove("interfaceRef")
                               .SetAttribute("interface", "IoC.Configuration.Tests.AutoService.Services.ActionValidator1");

                }), typeof(IAutoGeneratedServiceElement));
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void NonPublicService(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices")
                               .InsertChildElement(ConfigurationFileElementNames.AutoService)
                               .SetAttribute(ConfigurationFileAttributeNames.Interface, typeof(INonPublicInterface).FullName);

                }), typeof(IAutoGeneratedServiceElement));
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void MethodAddedMultipleTimes(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    var autoMethodElement = xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoMethod",
                        xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "GetDefaultProject");

                    var autoMethodElementClone = autoMethodElement.CloneNode(true);

                    autoMethodElement.ParentNode.AppendChild(autoMethodElementClone);

                }), typeof(IAutoGeneratedServiceMethodElement), typeof(IAutoGeneratedServiceElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodName(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.InterfaceRef) == "IProjectGuids")
                               .SelectChildElement("autoMethod",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "GetDefaultProject")
                               .SetAttribute(ConfigurationFileAttributeNames.Name, "NonExistentMethod");

                }), typeof(IAutoGeneratedServiceMethodElement));
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodReturnType(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                        xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.InterfaceRef) == "IProjectGuids")
                       .SelectChildElement("autoMethod",
                       (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "GetDefaultProject")
                       .Remove(ConfigurationFileAttributeNames.ReturnTypeRef)
                       .SetAttribute(ConfigurationFileAttributeNames.ReturnType, typeof(string).FullName);

                }), typeof(IAutoGeneratedServiceMethodElement));
        }

        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodReturnValue_1(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoMethod/default/object",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.TypeRef) == "Guid")
                               .SetAttribute(ConfigurationFileAttributeNames.Value, "1");

                }), typeof(IReturnValueElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodReturnValue_2(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoMethod/default/object",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.TypeRef) == "Guid")
                               .Remove(ConfigurationFileAttributeNames.TypeRef)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(System.Int32).FullName)
                               .SetAttribute(ConfigurationFileAttributeNames.Value, "1");

                }), typeof(IReturnValueElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodReturnValue_3(DiImplementationType diImplementationType)
        {

            Helpers.TestExpectedConfigurationParseException(() =>

                 LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                 {
                     xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoMethod/if",
                                    xmlElement => xmlElement.GetAttribute("parameter1") == "1" &&
                                    xmlElement.GetAttribute("parameter2") == "str1" &&
                                    xmlElement.SelectChildElement(ConfigurationFileElementNames.Collection) != null)
                                .RemoveChildElement(ConfigurationFileElementNames.Collection)
                                .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                                .SetAttributeValue(ConfigurationFileAttributeNames.Value, "1");
                 }), typeof(IReturnValueElement), typeof(IAutoGeneratedMemberReturnValuesSelectorElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodDeclaringInterfaceType_1(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Interface) == typeof(IMemberAmbiguityDemo).FullName)
                               .SelectChildElement("autoMethod",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "GetIntValues" &&
                                                   xmlElement.GetAttribute(ConfigurationFileAttributeNames.DeclaringInterface) == typeof(IMemberAmbiguityDemo_Parent3).FullName)
                               .SetAttribute(ConfigurationFileAttributeNames.DeclaringInterface, typeof(IInvalidDeclaringInterfaceTest).FullName);

                }), typeof(IAutoGeneratedServiceMethodElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodDeclaringInterfaceType_2(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Interface) == typeof(IMemberAmbiguityDemo).FullName)
                               .SelectChildElement("autoMethod",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "GetIntValues" &&
                                                   xmlElement.GetAttribute(ConfigurationFileAttributeNames.DeclaringInterface) == typeof(IMemberAmbiguityDemo_Parent3).FullName)
                               .SetAttribute(ConfigurationFileAttributeNames.DeclaringInterface, typeof(IMemberAmbiguityDemo_Parent1).FullName);

                }), typeof(IAutoGeneratedServiceMethodElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void AmbiguousMethod_DeclaringInterfaceIsNeeded(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Interface) == typeof(IMemberAmbiguityDemo).FullName)
                               .SelectChildElement("autoMethod",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "GetDbConnection" &&
                                                   xmlElement.GetAttribute(ConfigurationFileAttributeNames.DeclaringInterface) == typeof(IMemberAmbiguityDemo_Parent2).FullName)
                               .Remove(ConfigurationFileAttributeNames.DeclaringInterface);

                }), typeof(IAutoGeneratedServiceMethodElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodParameterName(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoMethod/methodSignature/object",
                                   xmlElement =>
                                       xmlElement.GetAttribute(ConfigurationFileAttributeNames.ParamName) == "projectGuid" &&
                                       xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == typeof(Guid).FullName)
                               .SetAttribute(ConfigurationFileAttributeNames.ParamName, "InvalidParamName");
                }), typeof(IMethodSignatureParameterElement), typeof(IMethodSignatureElement));

            // Lets test that removing the parameter name resolves the problem
            LoadConfigurationFile(diImplementationType, (xmlDocument) =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoMethod/methodSignature/object",
                               xmlElement =>
                                   xmlElement.GetAttribute(ConfigurationFileAttributeNames.ParamName) == "projectGuid" &&
                                   xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == typeof(Guid).FullName).Remove(ConfigurationFileAttributeNames.ParamName);
            });
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidMethodParameterType(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoMethod/methodSignature/object",
                                   xmlElement =>
                                       xmlElement.GetAttribute(ConfigurationFileAttributeNames.ParamName) == "projectGuid" &&
                                       xmlElement.GetAttribute(ConfigurationFileAttributeNames.Type) == typeof(Guid).FullName)
                               .SetAttribute(ConfigurationFileAttributeNames.Type, typeof(int).FullName);
                }), typeof(IAutoGeneratedServiceMethodElement));
        }


        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void PropertyAddedMultipleTimes(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    var autoPropertyElement = xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoProperty",
                        xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "Project1");

                    var autoPropertyElementClone = autoPropertyElement.CloneNode(true);
                    autoPropertyElement.ParentNode.AppendChild(autoPropertyElementClone);

                }), typeof(IAutoGeneratedServicePropertyElement), typeof(IAutoGeneratedServiceElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidPropertyName(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.InterfaceRef) == "IProjectGuids")
                               .SelectChildElement("autoProperty",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "Project1")
                               .SetAttribute(ConfigurationFileAttributeNames.Name, "NonExistentProperty");

                }), typeof(IAutoGeneratedServicePropertyElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidPropertyReturnType(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.InterfaceRef) == "IProjectGuids")
                               .SelectChildElement("autoProperty",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "Project1")
                               .Remove(ConfigurationFileAttributeNames.ReturnTypeRef)
                               .SetAttribute(ConfigurationFileAttributeNames.ReturnType, typeof(string).FullName);

                }), typeof(IAutoGeneratedServicePropertyElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidPropertyReturnValue_1(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoProperty/object",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.TypeRef) == "Guid")
                               .SetAttributeValue(ConfigurationFileAttributeNames.Value, "1");
                }), typeof(IReturnValueElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidPropertyReturnValue_2(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService/autoProperty/object",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.TypeRef) == "Guid")
                               .Remove(ConfigurationFileAttributeNames.TypeRef)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(int).FullName)
                               .SetAttributeValue(ConfigurationFileAttributeNames.Value, "1");
                }), typeof(IReturnValueElement));
        }



        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidPropertyDeclaringInterfaceType_1(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Interface) == typeof(IMemberAmbiguityDemo).FullName)
                               .SelectChildElement("autoProperty",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "DefaultDbConnection" &&
                                                   xmlElement.GetAttribute(ConfigurationFileAttributeNames.DeclaringInterface) == typeof(IMemberAmbiguityDemo_Parent1).FullName)
                               .SetAttribute(ConfigurationFileAttributeNames.DeclaringInterface, typeof(IInvalidDeclaringInterfaceTest).FullName);

                }), typeof(IAutoGeneratedServicePropertyElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidPropertyDeclaringInterfaceType_2(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Interface) == typeof(IMemberAmbiguityDemo).FullName)
                               .SelectChildElement("autoProperty",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "DefaultDbConnection" &&
                                                   xmlElement.GetAttribute(ConfigurationFileAttributeNames.DeclaringInterface) == typeof(IMemberAmbiguityDemo_Parent1).FullName)
                               .SetAttribute(ConfigurationFileAttributeNames.DeclaringInterface, typeof(IMemberAmbiguityDemo_Parent1_Parent).FullName);

                }), typeof(IAutoGeneratedServicePropertyElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void AmbiguousProperty_DeclaringInterfaceIsNeeded(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Interface) == typeof(IMemberAmbiguityDemo).FullName)
                               .SelectChildElement("autoProperty",
                                   (xmlElement) => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Name) == "DefaultDbConnection" &&
                                                   xmlElement.GetAttribute(ConfigurationFileAttributeNames.DeclaringInterface) == typeof(IMemberAmbiguityDemo_Parent1).FullName)
                               .Remove(ConfigurationFileAttributeNames.DeclaringInterface);

                }), typeof(IAutoGeneratedServicePropertyElement));
        }

        
        [TestCase(DiImplementationType.Autofac)]
        [TestCase(DiImplementationType.Ninject)]
        public void InvalidParameterNameInParameterValueElement(DiImplementationType diImplementationType)
        {
            Helpers.TestExpectedConfigurationParseException(() =>

                LoadConfigurationFile(diImplementationType, (xmlDocument) =>
                {
                    xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/autoGeneratedServices/autoService",
                                   xmlElement => xmlElement.GetAttribute(ConfigurationFileAttributeNames.Interface) == typeof(IAppInfoFactory).FullName)
                               .SelectChildElement("autoMethod/default/constructedValue/parameters/parameterValue")
                               .SetAttribute(ConfigurationFileAttributeNames.ParamName, "appId_Invalid");
                }), typeof(ParameterElement));
        }
    }

    internal interface INonPublicInterface
    {
        int GetInt();
    }

    public interface IInvalidDeclaringInterfaceTest
    {
        IReadOnlyList<int> GetIntValues(int param1, string param2);
    }
}
