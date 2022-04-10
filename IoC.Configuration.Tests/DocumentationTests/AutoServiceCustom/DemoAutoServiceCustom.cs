// Copyright (c) IoC.Configuration Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.

using System.IO;
using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.DiContainerBuilder.FileBased;
using NUnit.Framework;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary;

namespace IoC.Configuration.Tests.DocumentationTests.AutoServiceCustom;

[TestFixture]
public class DemoAutoServiceCustom
{
    private static IContainerInfo _containerInfo;

    [SetUp]
    public static void TestSetUp()
    {
        TestsHelper.SetupLogger();
        
        var fileBasedConfigurationParameters = new FileBasedConfigurationParameters(
            new FileBasedConfigurationFileContentsProvider(
                Path.Combine(Helpers.TestsEntryAssemblyFolder, @"DocumentationTests\AutoServiceCustom\DemoIoCConfiguration_autoServiceCustom.xml")),
            Helpers.TestsEntryAssemblyFolder,
            // LoadedAssembliesForTests is an implementation of ILoadedAssemblies that has a method 
            // "IEnumerable<Assembly> GetAssemblies()" that returns list of assemblies to add as references to
            // generate dynamic assembly.
            new LoadedAssembliesForTests())
        {
            AdditionalReferencedAssemblies = new []
            {
                // List additional assemblies that should be added to dynamically generated assembly as references
                Path.Combine(Helpers.GetTestFilesFolderPath(), @"DynamicallyLoadedDlls\TestProjects.DynamicallyLoadedAssembly1.dll"),
                Path.Combine(Helpers.GetTestFilesFolderPath(), @"DynamicallyLoadedDlls\TestProjects.DynamicallyLoadedAssembly2.dll")
            },
            AttributeValueTransformers = new[] {new FileFolderPathAttributeValueTransformer()},
            ConfigurationFileXmlDocumentLoaded = (sender, e) => 
                Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"))
        };
        
        _containerInfo = new DiContainerBuilder.DiContainerBuilder()
            .StartFileBasedDi(fileBasedConfigurationParameters, out _)
            .WithoutPresetDiContainer()
            .RegisterModules().Start();
    }

    [Test]
    public void Demo()
    {
        var simpleAutoImplementedInterface1 = _containerInfo.DiContainer.Resolve<ISimpleAutoImplementedInterface1>();
        Assert.AreEqual(10, simpleAutoImplementedInterface1.GetValue());
        
        var simpleAutoImplementedInterface2 = _containerInfo.DiContainer.Resolve<ISimpleAutoImplementedInterface2>();
        Assert.AreEqual(20, simpleAutoImplementedInterface2.GetSomeOtherValue());
    }
    
    [TearDown]
    public static void TestTeaDown()
    {
        _containerInfo.Dispose();
    }
}