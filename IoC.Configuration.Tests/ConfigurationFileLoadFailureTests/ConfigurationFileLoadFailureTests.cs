﻿// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.OnApplicationStart;
using IoC.Configuration.Tests.ConfigurationFileLoadFailureTests.TestClasses;
using JetBrains.Annotations;
using NUnit.Framework;
using OROptimizer;
using OROptimizer.Serializer;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using IoC.Configuration.AttributeValueTransformer;
using IoC.Configuration.DiContainerBuilder.FileBased;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests
{
    [TestFixture]
    public class ConfigurationFileLoadFailureTests
    {
        private const string ConfigurationFileName = "IoCConfiguration_Overview.xml";
    
        private string _configurationFileRelativePath = ConfigurationFileName;

        #region Member Functions
        [SetUp]
        public void TestInitialize()
        {
            _configurationFileRelativePath = ConfigurationFileName;

            TestsHelper.SetupLogger();
        }

        private IContainerInfo LoadConfigurationFile([CanBeNull] Action<XmlDocument> modifyConfigurationFileOnLoad)
        {
            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();

            var fileBasedConfigurationParameters = 
                new FileBasedConfigurationParameters(new FileBasedConfigurationFileContentsProvider(
                        Path.Combine(Helpers.TestsEntryAssemblyFolder, _configurationFileRelativePath)),
                Helpers.TestsEntryAssemblyFolder, new LoadedAssembliesForTests())
            {
                ConfigurationFileXmlDocumentLoaded = (sender, e) =>
                {
                    // Lets explicitly set the DiManager to Autofac. Since we are going to test failure, the Di manager implementation does not matter.
                    // However, this will give us predictability on what modules will be enabled.
                    Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"));
                    e.XmlDocument.SelectElement("/iocConfiguration/diManagers").SetAttributeValue(ConfigurationFileAttributeNames.ActiveDiManagerName, "Autofac");
                    modifyConfigurationFileOnLoad?.Invoke(e.XmlDocument);
                },
                AttributeValueTransformers = new IAttributeValueTransformer[]
                {
                    new FileFolderPathAttributeValueTransformer()
                }
            };

            return diContainerBuilder.StartFileBasedDi(fileBasedConfigurationParameters, out _)
                .WithoutPresetDiContainer()
                .AddAdditionalDiModules(new SuccessfulConfigurationLoadTests.SuccessfulConfigurationLoadTests.TestModule2())
                .RegisterModules()
                .Start();
        }

        #region TODO: Move the succes tests in this region to a separate success tests for replacing some configurations, and setup Autofac and Ninject tests.
        [Test]
        public void TestSuccessfulLoad_parameterSerializer_PresetParameterSerializersCanBeReplaced()
        {
            LoadConfigurationFile(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers/serializers/parameterSerializer",
                        x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                            .Equals(typeof(TypeBasedSimpleSerializerInt).FullName, StringComparison.Ordinal))
                    .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(IntParameterSerializer).FullName)
                    .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestSuccessfulLoadPluginModules_onPluginAssemblyCanBeUsedInPluginSection(bool useAssemblyAlias)
        {
            LoadConfigurationFile(xmlDocument =>
            {
                var moduleElement = xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/modules/module",
                        x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("ModulesForPlugin1.Autofac.AutofacModule1"))
                    .SetAttributeValue(ConfigurationFileAttributeNames.Type, "Modules.Autofac.AutofacModule1");

                if (useAssemblyAlias)
                    moduleElement.SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "modules");
            });
        }
        #endregion
        [Test]
        public void TestFailedLoad_additionalAssemblyProbingPaths_DuplicatePaths()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var firstProbingPath = xmlDocument.SelectElement("/iocConfiguration/additionalAssemblyProbingPaths/probingPath");

                xmlDocument.SelectElement("/iocConfiguration/additionalAssemblyProbingPaths")
                           .InsertChildElement(ConfigurationFileElementNames.ProbingPath)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Path,
                               firstProbingPath.GetAttribute(ConfigurationFileAttributeNames.Path));
            }, typeof(ProbingPath), typeof(AdditionalAssemblyProbingPaths));
        }

        [Test]
        public void TestFailedLoad_additionalAssemblyProbingPaths_InvalidPath()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/additionalAssemblyProbingPaths/probingPath")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Path, @"e:\InvalidPath");
            }, typeof(ProbingPath));
        }

        [Test]
        public void TestFailedLoad_appDataDir_PathInvalid()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/appDataDir")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Path, "Invalid path::");
            }, typeof(ApplicationDataDirectory));
        }

        [Test]
        public void TestFailedLoad_assemblies_DuplicateAliases()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var firstAssembly = xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly");

                // Lets sate the alias of the second assembly to the same value as in the first assembly.
                xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly", x => x != firstAssembly)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Alias,
                               firstAssembly.GetAttribute(ConfigurationFileAttributeNames.Alias));
            }, typeof(ConfigurationFile.Assembly), typeof(Assemblies));
        }

        [Test]
        public void TestFailedLoad_assemblies_DuplicateNames()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var firstAssembly = xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly");

                xmlDocument.SelectElement("/iocConfiguration/assemblies")
                           .InsertChildElement(ConfigurationFileElementNames.Assembly)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Alias, "UniqueAssemblyAliasForTest")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name,
                               firstAssembly.GetAttribute(ConfigurationFileAttributeNames.Name));
            }, typeof(ConfigurationFile.Assembly), typeof(Assemblies));
        }

        

        [Test]
        public void TestFailedLoad_assembly_AssemblyNameEndsWithDll()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var firstAssemby = xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly");
                xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name,
                               $"{Path.GetFileNameWithoutExtension(firstAssemby.GetAttribute(ConfigurationFileAttributeNames.Name))}.dll");
            }, typeof(ConfigurationFile.Assembly));
        }

        [Test]
        public void TestFailedLoad_assembly_AssemblyNotInPluginFolder()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var pluginName = "Plugin1";

                xmlDocument.ValidateElementExists("/iocConfiguration/plugins/plugin",
                    x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals(pluginName, StringComparison.Ordinal));

                xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Alias).Equals("oroptimizer_shared", StringComparison.Ordinal))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Plugin, pluginName);
            }, typeof(ConfigurationFile.Assembly));
        }

        [Test]
        public void TestFailedLoad_assembly_InvalidAssembmblyName_FailedToResolvedInProbingPaths()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/assemblies")
                           .InsertChildElement(ConfigurationFileElementNames.Assembly)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "NonExistentAssemblyName")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Alias, "NonExistentAssemblyAlias");
            }, typeof(ConfigurationFile.Assembly));
        }

        [Test]
        public void TestFailedLoad_assembly_InvalidOverrideDirectory()
        {
            using (new IoCServiceFactoryStaticContextMockSwitch())
            {
                TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
                {
                    var assemblyName = "TestProjects.DynamicallyLoadedAssembly1";
                    xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly",
                            xmlElement => xmlElement.GetAttribute("name") == assemblyName)
                        .SetAttributeValue("overrideDirectory", @"TestFiles\ThirdPartyLibs");
                    IoCServiceFactoryMock.SetFailedAssemblies(new[] { assemblyName });
                }, typeof(ConfigurationFile.Assembly));
            }
        }

        [Test]
        public void TestFailedLoad_assembly_AssemblyLoadFails()
        {
            using (new IoCServiceFactoryStaticContextMockSwitch())
            {
                TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
                {
                    var assemblyName = "OROptimizer.Shared";
                    var invalidAssemblyName = $"{assemblyName}_Invalid";
                    xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly",
                            xmlElement => xmlElement.GetAttribute("name") == assemblyName )
                        .SetAttributeValue("name", invalidAssemblyName);
                    IoCServiceFactoryMock.SetFailedAssemblies(new[] { invalidAssemblyName });
                }, typeof(ConfigurationFile.Assembly));
            }
        }

        [Test]
        public void TestFailedLoad_assembly_InvalidOwnerPlugin()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/assemblies/assembly")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Plugin, "NonExistentPlugin");
            }, typeof(ConfigurationFile.Assembly));
        }

        
        [Test]
        public void TestFailedLoad_DiManagerElement_InvalidConstructorParameter()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/diManagers/diManager",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("Autofac", StringComparison.Ordinal))
                           .InsertChildElement(ConfigurationFileElementNames.Parameters)
                           .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "param1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10");
            }, typeof(DiManagerElement));
        }

        [Test]
        public void TestFailedLoad_DiManagerElement_InvalidDiManagerAssemblyName()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/diManagers/diManager",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("Autofac"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "nonexistent_assembly_alias");
            }, typeof(DiManagerElement));
        }

        [Test]
        public void TestFailedLoad_DiManagerElement_InvalidDiManagerType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/diManagers/diManager",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("Autofac"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(InvalidDiManagar).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            }, typeof(DiManagerElement));
        }

        [Test]
        public void TestFailedLoad_DiManagerElement_NonExistentDiManagerType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/diManagers/diManager",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("Autofac"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "NonExistentType");
            }, typeof(DiManagerElement));
        }

        [Test]
        public void TestFailedLoad_DiManagersElement_DuplicateDiManagerNames()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/diManagers")
                           .InsertChildElement(ConfigurationFileElementNames.DiManager)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "Autofac")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "IoC.Configuration.Autofac.AutofacDiManager")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "autofac_ext");
            }, typeof(DiManagerElement), typeof(DiManagersElement));
        }

        [Test]
        public void TestFailedLoad_DiManagersElement_InvalidActiveDiManagerName()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/diManagers")
                           .SetAttributeValue(ConfigurationFileAttributeNames.ActiveDiManagerName, "NonExistentDiManagerName");
            }, typeof(DiManagersElement));
        }

        [Test]
        public void TestFailedLoad_implementation_InvalidConstructorParameters()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/service/implementation",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                                     .Equals(typeof(Interface2_Impl1).FullName))
                           .SelectChildElement(ConfigurationFileElementNames.Parameters)
                           .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "InvalidParameter")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10");
            }, typeof(TypeBasedServiceImplementationElement));
        }

        [Test]
        public void TestFailedLoad_implementation_InvalidInjectedPropertyType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/service/implementation",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                                     .Equals(typeof(Interface2_Impl3).FullName))
                           .SelectChildElement(ConfigurationFileElementNames.InjectedProperties)
                           .RemoveChildElement(ConfigurationFileElementNames.ValueDouble,
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("Property2"))
                           .InsertChildElement(ConfigurationFileElementNames.ValueBoolean)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "Property2")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "true");
            }, typeof(InjectedPropertyElement), typeof(TypeBasedServiceImplementationElement));
        }

        private void TestFailedLoad_implementation_InvalidSelfBoundServiceType(Type selfBoundServiceType)
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                Assert.IsTrue(selfBoundServiceType.IsInterface ||
                              selfBoundServiceType.IsAbstract || !selfBoundServiceType.GetConstructors().Where(x => x.IsPublic).Any());

                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services")
                           .InsertChildElement(ConfigurationFileElementNames.SelfBoundService)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, selfBoundServiceType.FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "shared_services")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Scope, "singleton");
            }, typeof(SelfBoundServiceElement));
        }


        private void TestFailedLoad_implementation_InvalidServiceImplementationType(Type interfaceType, Type invalidImplementationType, bool expectsServiceElementInError)
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                Assert.IsTrue(!interfaceType.IsAssignableFrom(invalidImplementationType) || invalidImplementationType.IsInterface ||
                              invalidImplementationType.IsAbstract || !invalidImplementationType.GetConstructors().Where(x => x.IsPublic).Any());

                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/service",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals(interfaceType.FullName))
                           .SelectChildElement(ConfigurationFileElementNames.Implementation)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, invalidImplementationType.FullName);
            }, typeof(TypeBasedServiceImplementationElement), expectsServiceElementInError ? typeof(ServiceElement) : null);
        }


        [Test]
        public void TestFailedLoad_implementation_NonExistentInjectedProperty()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/service/implementation",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                                     .Equals(typeof(Interface2_Impl3).FullName))
                           .SelectChildElement(ConfigurationFileElementNames.InjectedProperties)
                           .InsertChildElement(ConfigurationFileElementNames.ValueDouble)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "NonExistentProperty")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10.2");
            }, typeof(InjectedPropertyElement), typeof(TypeBasedServiceImplementationElement));
        }

        [Test]
        public void TestFailedLoad_modules_DuplicateModuleNames()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.AddSiblingClone("/iocConfiguration/dependencyInjection/modules/module",
                    x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("Modules.Autofac.AutofacModule1"));
            }, typeof(ModuleElement), typeof(ModulesElement));
        }

        [Test]
        public void TestFailedLoad_modules_InvalidConstructorParameters()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/modules/module",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("Modules.Autofac.AutofacModule1"))
                           .SelectChildElement(ConfigurationFileElementNames.Parameters)
                           .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "InvalidParameter")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10");
            }, typeof(ModuleElement));
        }

        /// <summary>
        ///     Module type can be either <see cref="IDiModule" />, or <see cref="IDiManager.ModuleType" /> of one of DI managers
        ///     defined
        ///     in diManagers element.
        /// </summary>
        [Test]
        public void TestFailedLoad_modules_InvalidNativeModuleType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/modules")
                           .InsertChildElement(ConfigurationFileElementNames.Module)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(Class1).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            }, typeof(ModuleElement));
        }

        [Test]
        public void TestFailedLoad_modules_ModuleIsInPluginAssembly()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/modules/module",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("Modules.Autofac.AutofacModule1"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "ModulesForPlugin1.Autofac.AutofacModule1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "modules_plugin1");
            }, typeof(ModuleElement));
        }

        [Test]
        public void TestFailedLoad_modules_NonExistentModuleAssembly()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/modules/module",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("Modules.Autofac.AutofacModule1"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "NonExistentAssemblyAlias");
            }, typeof(ModuleElement));
        }

        [Test]
        public void TestFailedLoad_modules_NonExistentModuleType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/modules/module",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("Modules.Autofac.AutofacModule1"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "NonExistentType");
            }, typeof(ModuleElement));
        }

        [Test]
        public void TestFailedLoad_parameter_InvalidParameterInServiceImplementation()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Plugin)
                                     .Equals("Plugin1", StringComparison.Ordinal))
                           .SelectChildElement("dependencyInjection/services/service/implementation",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("TestPluginAssembly1.Implementations.Room"))
                           .SelectChildElement(ConfigurationFileElementNames.Parameters)
                           .InsertChildElement(ConfigurationFileElementNames.ValueDouble)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "InvalidParamName")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10.2");
            }, typeof(TypeBasedServiceImplementationElement));
        }

        [Test]
        public void TestFailedLoad_parameter_NoSerializerForTheObject()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Plugin)
                                     .Equals("Plugin1", StringComparison.Ordinal))
                           .SelectChildElement("dependencyInjection/services/service/implementation",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("TestPluginAssembly1.Implementations.Room"))
                           .SelectChildElement("parameters/object", x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("door1"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(Class1).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            }, typeof(ParameterElement));
        }

        [Test]
        public void TestFailedLoad_parameter_NoSuchParameterInServiceImplementationConstructor()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Plugin)
                                     .Equals("Plugin1", StringComparison.Ordinal))
                           .SelectChildElement("dependencyInjection/services/service/implementation",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("TestPluginAssembly1.Implementations.Room"))
                           .SelectChildElement("parameters/object", x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("door1"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(Class1).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            }, typeof(ParameterElement));
        }

        [Test]
        public void TestFailedLoad_parameter_SerializerFailsToDeserializeTheObject()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Plugin)
                                     .Equals("Plugin1", StringComparison.Ordinal))
                           .SelectChildElement("dependencyInjection/services/service/implementation",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("TestPluginAssembly1.Implementations.Room"))
                           .SelectChildElement("parameters/object", x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("door1"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "invalid door value");
            }, typeof(ParameterElement));
        }

        

        [Test]
        public void TestFailedLoad_parameterSerializers_DefaultTypeIfMissingBothAttributes()
        {
            using (var containerInfo = LoadConfigurationFile(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers")
                           .Remove(ConfigurationFileAttributeNames.Assembly)
                           .Remove(ConfigurationFileAttributeNames.SerializerAggregatorType);
            }))
            {
                var typeBasedSimpleSerializerAggregator = containerInfo.DiContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
                Assert.AreEqual(typeof(TypeBasedSimpleSerializerAggregator), typeBasedSimpleSerializerAggregator.GetType(), "If type attributes are omitted, default serializer should be used.");
            }
        }

        [Test]
        public void TestFailedLoad_parameterSerializers_InvalidAssemblyAlias()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers",
                               x =>
                                   x.GetAttribute(ConfigurationFileAttributeNames.SerializerAggregatorType).Equals(typeof(TypeBasedSimpleSerializerAggregator).FullName, StringComparison.Ordinal))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "assembly_invalid");
            }, typeof(ParameterSerializers));
        }

        [Test]
        public void TestFailedLoad_parameterSerializers_InvalidParameterSerializersTypeConstructorParameter()
        {
            Action<XmlDocument, bool> replaceSerializer = (xmlDocument, isInvalidParamTest) =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers")
                           .SetAttributeValue(ConfigurationFileAttributeNames.SerializerAggregatorType, typeof(TypeBasedSimpleSerializerAggregatorForTest).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");

                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers")
                           .InsertChildElement(ConfigurationFileElementNames.Parameters, 0)
                           .InsertChildElement(isInvalidParamTest ? "double" : "int32")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "param1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10");
            };

            // Make sure that configuration loads successfully with valid parameter type
            using (var containerInfo = LoadConfigurationFile(xmlDocument => { replaceSerializer(xmlDocument, false); }))
            {
                var typeBasedSimpleSerializerAggregator =
                    (TypeBasedSimpleSerializerAggregatorForTest) containerInfo.DiContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
                Assert.AreEqual(10, typeBasedSimpleSerializerAggregator.Property1);
            }

            // Now make sure the configuration file load fails with invalid parameter type

            TestFailedLoadConfigurationFileWithStandardError(xmlDocument => { replaceSerializer(xmlDocument, true); }, typeof(ParameterSerializers));
        }

        [Test]
        public void TestFailedLoad_parameterSerializers_InvalidValueOfParameterSerializersType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.SerializerAggregatorType).Equals(typeof(TypeBasedSimpleSerializerAggregator).FullName, StringComparison.Ordinal))
                           .SetAttributeValue(ConfigurationFileAttributeNames.SerializerAggregatorType, typeof(InvalidTypeBasedSimpleSerializerAggregator).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            }, typeof(ParameterSerializers));
        }

        [Test]
        public void TestFailedLoad_parameterSerializers_InvalidAssemblyAttributeValue_assembly()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "modules");
            }, typeof(ParameterSerializers));
        }

      

        [Test]
        public void TestFailedLoad_parameterSerializers_MissingAttribute_serializerAggregatorType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers")
                           .RemoveAttribute(ConfigurationFileAttributeNames.SerializerAggregatorType);
            }, typeof(ParameterSerializers));
        }

        [Test]
        public void TestFailedLoad_parameterSerializers_NonExistentTypeAsAValueofParameterSerializersType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.SerializerAggregatorType).Equals(typeof(TypeBasedSimpleSerializerAggregator).FullName, StringComparison.Ordinal))
                           .SetAttributeValue(ConfigurationFileAttributeNames.SerializerAggregatorType, $"{typeof(TypeBasedSimpleSerializerAggregator).FullName}_{GlobalsCoreAmbientContext.Context.GenerateUniqueId()}");
            }, typeof(ParameterSerializers));
        }

        [Test]
        public void TestFailedLoad_parameterSerializers_UsesDisabledPlugin()
        {
            TestFailedLoadConfigurationFile<DisabledPluginTypeUsedConfigurationParseException>(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/plugins/plugin",
                    (element => element.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("Plugin1", StringComparison.Ordinal)))
                    .SetAttributeValue(ConfigurationFileAttributeNames.Enabled, "false");
            }, 
            typeof(ParameterSerializer), null, 
            (e) =>
            {
                Assert.IsTrue(e is ConfigurationParseException parseException && 
                              parseException.ConfigurationFileElement is IParameterSerializer parameterSerializer &&
                              parameterSerializer.ValueTypeInfo.TypeCSharpFullName.Equals("TestPluginAssembly1.Implementations.DoorSerializer", StringComparison.Ordinal));
            });

        }

        [Test]
        public void TestFailedLoad_ParameterSerializersCollection_multipleSerializersForSameType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/parameterSerializers/serializers")
                           .InsertChildElement(ConfigurationFileElementNames.ParameterSerializer)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "TestPluginAssembly1.Implementations.DoorSerializer2")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "pluginassm1");
            }, typeof(ParameterSerializer), typeof(ParameterSerializersCollection));
        }

        [Test]
        public void TestFailedLoad_PluginModules_DuplicateModuleNames()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.AddSiblingClone("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/modules/module",
                    x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("ModulesForPlugin1.Autofac.AutofacModule1"));
            }, typeof(ModuleElement), typeof(ModulesElement));
        }

        [Test]
        public void TestFailedLoad_PluginModules_InvalidConstructorParameters()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/modules/module",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("ModulesForPlugin1.Autofac.AutofacModule1"))
                           .SelectChildElement(ConfigurationFileElementNames.Parameters)
                           .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "InvalidParameter")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10");
            }, typeof(ModuleElement));
        }

        /// <summary>
        ///     Module type can be either <see cref="IDiModule" />, or <see cref="IDiManager.ModuleType" /> of one of DI managers
        ///     defined
        ///     in diManagers element.
        /// </summary>
        [Test]
        public void TestFailedLoad_PluginModules_InvalidNativeModuleType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/modules")
                           .InsertChildElement(ConfigurationFileElementNames.Module)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "TestPluginAssembly1.Implementations.Class1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "pluginassm1");
            }, typeof(ModuleElement));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestFailedLoad_PluginModules_ModuleBelongsToDifferentPlugin(bool useAssemblyAlias)
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var xmlElement = xmlDocument.SelectElement(
                        "/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/modules/module",
                        x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                            .Equals("ModulesForPlugin1.Autofac.AutofacModule1"))
                    .SetAttributeValue(ConfigurationFileAttributeNames.Type,
                        "ModulesForPlugin2.Autofac.AutofacModule1");

                if (useAssemblyAlias)
                    xmlElement.SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "modules_plugin2");
            }, typeof(ModuleElement));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestFailedLoad_PluginModules_PluginModuleIsUSedInNonPluginArea(bool useAssemblyAlias)
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var modulesElement = xmlDocument.SelectElement(
                        "/iocConfiguration/dependencyInjection/modules");

                var moduleElement = modulesElement.InsertChildElement(ConfigurationFileElementNames.Module);
                moduleElement.SetAttributeValue(ConfigurationFileAttributeNames.Type,
                    "ModulesForPlugin1.Autofac.AutofacModule1");

                if (useAssemblyAlias)
                    moduleElement.SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "modules_plugin1");

                moduleElement.InsertChildElement("parameters")
                    .InsertChildElement("int32")
                    .SetAttributeValue(ConfigurationFileAttributeNames.Name, "param1")
                    .SetAttributeValue(ConfigurationFileAttributeNames.Value, "102");

                
            }, typeof(ModuleElement));
        }

        [Test]
        public void TestFailedLoad_PluginModules_NonExistentModuleAssembly()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/modules/module",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("ModulesForPlugin1.Autofac.AutofacModule1"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "NonExistentAssemblyAlias");
            }, typeof(ModuleElement));
        }

        [Test]
        public void TestFailedLoad_PluginModules_NonExistentModuleType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/modules/module",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("ModulesForPlugin1.Autofac.AutofacModule1"))
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "NonExistentType");
            }, typeof(ModuleElement));
        }

        [Test]
        public void TestFailedLoad_plugins_DuplicatePluginNames()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/plugins")
                           .InsertChildElement(ConfigurationFileElementNames.Plugin)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "Plugin1");
            }, typeof(PluginElement), typeof(Plugins));
        }

        [Test]
        public void TestFailedLoad_plugins_PluginIsNotDefinedInPluginsSetupSections()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/plugins")
                           .InsertChildElement(ConfigurationFileElementNames.Plugin)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "Plugin4");
            }, typeof(PluginElement), typeof(Plugins));
        }

        [Test]
        public void TestFailedLoad_plugins_PluginsDirectoryPathInvalid()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/plugins")
                           .SetAttribute(ConfigurationFileAttributeNames.PluginsDirPath, "invalid");
            }, typeof(Plugins));
        }

        [Test]
        public void TestFailedLoad_plugins_PluginsDirectoryPathMissing()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/plugins")
                           .RemoveAttribute(ConfigurationFileAttributeNames.PluginsDirPath);
            }, typeof(PluginElement), typeof(Plugins));
        }

        [Test]
        public void TestFailedLoad_Plugins_services_DuplicateServiceTypes()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.AddSiblingClone("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/services/service",
                    x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("TestPluginAssembly1.Interfaces.IDoor"));
            }, typeof(IServiceElement), typeof(Services));
        }

        [Test]
        public void TestFailedLoad_PluginSettings_DuplicateSettingNames()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Plugin).Equals("Plugin1"))
                           .SelectChildElement(ConfigurationFileElementNames.Settings)
                           .InsertChildElement(ConfigurationFileElementNames.ValueDouble)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "Int32Setting1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "15.2");
            }, typeof(SettingElement), typeof(SettingsElement));
        }

        [Test]
        public void TestFailedLoad_PluginSettings_RequiredSettingIsMissing()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Plugin).Equals("Plugin1"))
                           .SelectChildElement(ConfigurationFileElementNames.Settings)
                           .RemoveChildElement(ConfigurationFileElementNames.ValueInt32, x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("Int32Setting1"));
            }, typeof(SettingsElement));
        }

        [Test]
        public void TestFailedLoad_PluginSettings_RequiredSettingTypeIsInvalid()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var settingName = "Int32Setting1";

                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Plugin).Equals("Plugin1"))
                           .SelectChildElement(ConfigurationFileElementNames.Settings)
                           .RemoveChildElement(ConfigurationFileElementNames.ValueInt32,
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals(settingName))
                           .InsertChildElement(ConfigurationFileElementNames.ValueString)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, settingName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "some text");
            }, typeof(SettingElement), typeof(SettingsElement));
        }

        [Test]
        public void TestFailedLoad_SchemaViolation()
        {
            TestFailedLoadConfigurationFile<Exception>(xmlDocument => { xmlDocument.SelectElement("/iocConfiguration").InsertChildElement("someInvalidElement"); }, null);
        }

        [Test]
        public void TestFailedLoad_selfBoundService_InvalidConstructorParameters()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/selfBoundService",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                                     .Equals("DynamicallyLoadedAssembly1.Implementations.SelfBoundService1"))
                           .SelectChildElement(ConfigurationFileElementNames.Parameters)
                           .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "InvalidParameter")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10");
            }, typeof(SelfBoundServiceElement));
        }

        [Test]
        public void TestFailedLoad_selfBoundService_InvalidInjectedPropertyType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/selfBoundService",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                                     .Equals("DynamicallyLoadedAssembly1.Implementations.SelfBoundService2"))
                           .SelectChildElement(ConfigurationFileElementNames.InjectedProperties)
                           .RemoveChildElement(ConfigurationFileElementNames.ValueInt32,
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("Property1"))
                           .InsertChildElement(ConfigurationFileElementNames.ValueDouble)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "Property1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "13.1");
            }, typeof(InjectedPropertyElement), typeof(SelfBoundServiceElement));
        }

        [Test]
        public void TestFailedLoad_selfBoundService_InvalidSelfBoundServiceType_AbstractClass()
        {
            TestFailedLoad_implementation_InvalidSelfBoundServiceType(typeof(Interface9_InvalidImpl_AbstrClass));
        }

        [Test]
        public void TestFailedLoad_selfBoundService_InvalidSelfBoundServiceType_Interface()
        {
            TestFailedLoad_implementation_InvalidSelfBoundServiceType(typeof(Interface9_InvalidImpl_Interface));
        }

        [Test]
        public void TestFailedLoad_selfBoundService_InvalidSelfBoundServiceType_NoPublicConstructors()
        {
            TestFailedLoad_implementation_InvalidSelfBoundServiceType(typeof(Interface9_InvalidImpl_NoPublicConstructor));
        }

        [Test]
        public void TestFailedLoad_selfBoundService_NonExistentInjectedProperty()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/selfBoundService",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                                     .Equals("DynamicallyLoadedAssembly1.Implementations.SelfBoundService2"))
                           .SelectChildElement(ConfigurationFileElementNames.InjectedProperties)
                           .InsertChildElement(ConfigurationFileElementNames.ValueDouble)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "NonExistentProperty")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10.2");
            }, typeof(InjectedPropertyElement), typeof(SelfBoundServiceElement));
        }

        [Test]
        public void TestFailedLoad_service_InvalidServiceImplementation_AbstractClass()
        {
            
            TestFailedLoad_implementation_InvalidServiceImplementationType(typeof(IInterface9), typeof(Interface9_InvalidImpl_AbstrClass), false);
        }

        [Test]
        public void TestFailedLoad_service_InvalidServiceImplementation_Interface()
        {
            TestFailedLoad_implementation_InvalidServiceImplementationType(typeof(IInterface9), typeof(Interface9_InvalidImpl_Interface), false);
        }

        [Test]
        public void TestFailedLoad_service_InvalidServiceImplementation_NoPublicConstructors()
        {
            TestFailedLoad_implementation_InvalidServiceImplementationType(typeof(IInterface9), typeof(Interface9_InvalidImpl_NoPublicConstructor), false);
        }

        [Test]
        public void TestFailedLoad_service_InvalidServiceImplementationType()
        {
            TestFailedLoad_implementation_InvalidServiceImplementationType(typeof(IInterface9), typeof(Interface1_Impl1), true);
        }

        [Test]
        public void TestFailedLoad_service_MultipleImplementationsForServiceWithRegisterIfNotRegisteredOn()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services/service",
                               x => x.GetAttribute(ConfigurationFileAttributeNames.Type)
                                     .Equals(typeof(IInterface5).FullName))
                           .SetAttributeValue(ConfigurationFileAttributeNames.RegisterIfNotRegistered, "true");
            }, typeof(ServiceElement));
        }

        [Test]
        public void TestFailedLoad_service_NonPluginSelfboundServiceInPluginSection()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/services")
                           .InsertChildElement(ConfigurationFileElementNames.SelfBoundService)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(Interface1_Impl1).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "shared_services")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Scope, "singleton");
            }, typeof(SelfBoundServiceElement));
        }

        [Test]
        public void TestFailedLoad_service_NonPluginServiceInPluginSection()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/services")
                           .InsertChildElement(ConfigurationFileElementNames.Service)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(IInterface1).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "shared_services")
                           .InsertChildElement(ConfigurationFileElementNames.Implementation)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(Interface1_Impl1).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "shared_services")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Scope, "singleton");
            }, typeof(ServiceElement));
        }

        [Test]
        public void TestFailedLoad_service_PluginSelfboundServiceInNonPluginSection()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services")
                           .InsertChildElement(ConfigurationFileElementNames.SelfBoundService)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "TestPluginAssembly1.Implementations.Plugin1_Interface1_Impl1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "pluginassm1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Scope, "singleton");
            }, typeof(SelfBoundServiceElement));
        }

        [Test]
        public void TestFailedLoad_service_PluginServiceInNonPluginSection()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services")
                           .InsertChildElement(ConfigurationFileElementNames.Service)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "TestPluginAssembly1.Interfaces.IPlugin1_Interface1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "pluginassm1")
                           .InsertChildElement(ConfigurationFileElementNames.Implementation)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "TestPluginAssembly1.Implementations.Plugin1_Interface1_Impl1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "pluginassm1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Scope, "singleton");
            }, typeof(ServiceElement));
        }

        [Test]
        public void TestFailedLoad_services_DuplicateServiceTypes()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.AddSiblingClone("/iocConfiguration/dependencyInjection/services/service",
                    x => x.GetAttribute(ConfigurationFileAttributeNames.Type).Equals("DynamicallyLoadedAssembly1.Interfaces.IInterface1"));
            }, typeof(IServiceElement), typeof(Services));
        }

        [Test]
        public void TestFailedLoad_services_IPlugin_IsNotAllowedInServicesElement()
        {
            TestInvalidServiceTypeInServiceElement(typeof(IPlugin), "TestPluginAssembly1.Implementations.Plugin1",
                "pluginassm1", new[] {ConfigurationFileElementNames.ValueInt64}, new[] {"25"});
        }

        [Test]
        public void TestFailedLoad_services_ISettingsRequestor_IsNotAllowedInServicesElement()
        {
            TestInvalidServiceTypeInServiceElement(typeof(ISettingsRequestor), "SharedServices.FakeSettingsRequestor",
                "shared_services", null, null);
        }

        [Test]
        public void TestFailedLoad_services_IStartupAction_IsNotAllowedInServicesElement()
        {
            TestInvalidServiceTypeInServiceElement(typeof(IStartupAction), "DynamicallyLoadedAssembly1.Implementations.StartupAction1",
                "dynamic1", null, null);
        }

        [Test]
        public void TestFailedLoad_settings_DuplicateSettingNames()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/settings")
                           .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "MaxCharge")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "15");
            }, typeof(SettingElement), typeof(SettingsElement));
        }

        [Test]
        public void TestFailedLoad_settings_RequiredSettingIsMissing()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/settings/double",
                    x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals("MaxCharge")).Remove();
            }, typeof(SettingsElement));
        }

        [Test]
        public void TestFailedLoad_settings_RequiredSettingTypeIsInvalid()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var settingName = "MaxCharge";
                xmlDocument.SelectElement("/iocConfiguration/settings")
                           .RemoveChildElement(ConfigurationFileElementNames.ValueDouble, x => x.GetAttribute(ConfigurationFileAttributeNames.Name).Equals(settingName))
                           .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, settingName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10");
            }, typeof(SettingElement), typeof(SettingsElement));
        }

        [Test]
        public void TestFailedLoad_settingsRequestor_AbstractISettingsRequestorImplementation()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/settingsRequestor")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(SettingsRequestorAbstr).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            }, typeof(SettingsRequestorImplementationElement));
        }

        [Test]
        public void TestFailedLoad_settingsRequestor_InvalidConstructorParameters()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/settingsRequestor")
                           .InsertChildElement(ConfigurationFileElementNames.Parameters)
                           .InsertChildElement(ConfigurationFileElementNames.ValueInt32)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Name, "param1")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Value, "10");
            }, typeof(SettingsRequestorImplementationElement));
        }

        [Test]
        public void TestFailedLoad_settingsRequestor_InvalidSettingRequestorType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/settingsRequestor")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, typeof(Class1).FullName)
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            }, typeof(SettingsRequestorImplementationElement));
        }

        [Test]
        public void TestFailedLoad_settingsRequestor_ISettingsRequestorImplementationInPluginAssembly()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/settingsRequestor")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "TestPluginAssembly1.Implementations.InvalidSettingsRequestorInPluginAssembly")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "pluginassm1");
            }, typeof(SettingsRequestorImplementationElement));
        }

        [Test]
        public void TestFailedLoad_settingsRequestor_NonExistentSettingRequestorAssembly()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/settingsRequestor")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "NonExistentClass")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "NonExistentAssemblyAlias");
            }, typeof(SettingsRequestorImplementationElement));
        }

        [Test]
        public void TestFailedLoad_settingsRequestor_NonExistentSettingRequestorType()
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                xmlDocument.SelectElement("/iocConfiguration/settingsRequestor")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Type, "NonExistentClass")
                           .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "tests");
            }, typeof(SettingsRequestorImplementationElement));
        }


        private void TestFailedLoadConfigurationFile([CanBeNull] Action<XmlDocument> modifyConfigurationFileOnLoad,
                                                                         [CanBeNull] Type expectedConfigurationFileElementTypeAtError,
                                                                         [CanBeNull] Type expectedParentConfigurationFileElementTypeAtError = null,
                                                                         [CanBeNull] Action<Exception> doAdditionalTests = null)
        {
            TestFailedLoadConfigurationFile<ConfigurationParseException>(modifyConfigurationFileOnLoad,
                expectedConfigurationFileElementTypeAtError, expectedParentConfigurationFileElementTypeAtError, doAdditionalTests);
        }
        private void TestFailedLoadConfigurationFile<TExpectedException>([CanBeNull] Action<XmlDocument> modifyConfigurationFileOnLoad,
                                                     [CanBeNull] Type expectedConfigurationFileElementTypeAtError,
                                                     [CanBeNull] Type expectedParentConfigurationFileElementTypeAtError = null,
                                                     [CanBeNull] Action<Exception> doAdditionalTests = null) where TExpectedException : Exception
        {
            Helpers.TestExpectedConfigurationParseException<TExpectedException>(() =>
            {
                IContainerInfo containerInfo = null;
                try
                {
                    containerInfo = LoadConfigurationFile(modifyConfigurationFileOnLoad);
                }
                finally
                {
                    if (containerInfo != null)
                        containerInfo.Dispose();
                }
            },
            expectedConfigurationFileElementTypeAtError, expectedParentConfigurationFileElementTypeAtError, true, doAdditionalTests);
        }


        private void TestFailedLoadConfigurationFileWithStandardError([NotNull] Action<XmlDocument> modifyConfigurationFileOnLoad,
                                                                      [NotNull] Type expectedConfigurationFileElementTypeAtError,
                                                                      [CanBeNull] Type expectedParentConfigurationFileElementTypeAtError = null)
        {
            TestFailedLoadConfigurationFile(modifyConfigurationFileOnLoad, expectedConfigurationFileElementTypeAtError, expectedParentConfigurationFileElementTypeAtError);
        }


        private void TestInvalidServiceTypeInServiceElement(Type serviceType, string implementationType, string implementationAssembly, string[] parameterElements, string[] parameterValues)
        {
            TestFailedLoadConfigurationFileWithStandardError(xmlDocument =>
            {
                var implementationElement = xmlDocument.SelectElement("/iocConfiguration/dependencyInjection/services")
                                                       .InsertChildElement(ConfigurationFileElementNames.Service)
                                                       .SetAttributeValue(ConfigurationFileAttributeNames.Type, serviceType.FullName)
                                                       .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, "ioc_config")
                                                       .InsertChildElement(ConfigurationFileElementNames.Implementation)
                                                       .SetAttributeValue(ConfigurationFileAttributeNames.Type, implementationType)
                                                       .SetAttributeValue(ConfigurationFileAttributeNames.Assembly, implementationAssembly)
                                                       .SetAttributeValue(ConfigurationFileAttributeNames.Scope, "singleton");

                if (parameterElements?.Length > 0)
                {
                    var parametersElement = implementationElement.InsertChildElement(ConfigurationFileElementNames.Parameters);
                    for (var i = 0; i < parameterElements.Length; ++i)
                        parametersElement.InsertChildElement(parameterElements[i])
                                         .SetAttributeValue(ConfigurationFileAttributeNames.Name, $"param{i + 1}")
                                         .SetAttributeValue(ConfigurationFileAttributeNames.Value, parameterValues[i]);
                }
            }, typeof(IServiceElement), typeof(Services));
        }

        [Test]
        public void TestSuccessfulLoad()
        {
            ValidateLoadConfigurationFileSuccess(null);
        }

        private void ValidateLoadConfigurationFileSuccess([CanBeNull] Action<XmlDocument> modifyConfigurationFileOnLoad)
        {
            using (var containerInfo = LoadConfigurationFile(modifyConfigurationFileOnLoad))
            {
                Assert.IsNotNull(containerInfo);

                var resolvedInstance = containerInfo.DiContainer.Resolve<IPluginDataRepository>();

                Assert.IsNotNull(resolvedInstance);
            }
        }
        #endregion
    }
}