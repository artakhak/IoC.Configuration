using System.IO;
using IoC.Configuration.DiContainerBuilder.FileBased;
using NUnit.Framework;
using OROptimizer.Utilities.Xml;
using TestsSharedLibrary;

namespace IoC.Configuration.Tests.DocumentationTests.ReferencesInDynamicAssembly;

[TestFixture]
public class FileBasedConfigurationParameters_AdditionalReferencedAssemblies
{
    [Test]
    public void FileBasedConfigurationParameters_AdditionalReferencedAssemblies_Demo()
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
        
        var containerInfo = new DiContainerBuilder.DiContainerBuilder()
            .StartFileBasedDi(fileBasedConfigurationParameters, out _)
            .WithoutPresetDiContainer()
            .RegisterModules().Start();

        var autoImplementedInterfaceInstance = containerInfo.DiContainer.Resolve<IoC.Configuration.Tests.DocumentationTests.AutoServiceCustom.ISimpleAutoImplementedInterface1>();
        Assert.AreEqual(10, autoImplementedInterfaceInstance.GetValue());
    }
}