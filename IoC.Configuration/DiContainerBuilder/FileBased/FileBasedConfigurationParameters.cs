// This software is part of the IoC.Configuration library
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

using System.Collections.Generic;
using IoC.Configuration.AttributeValueTransformer;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    /// <summary>
    /// Parameters used for constructing and initializing an instance of <see cref="FileBasedConfiguration"/>.
    /// </summary>
    public sealed class FileBasedConfigurationParameters
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileBasedConfiguration" /> class.
        /// </summary>
        /// <param name="configurationFileContentsProvider">
        ///     The configuration file contents provider. An example implementation of
        ///     <see cref="IConfigurationFileContentsProvider" /> implementation is
        ///     <see cref="FileBasedConfigurationFileContentsProvider" />
        /// </param>
        /// <param name="entryAssemblyFolder">
        ///     The location where the executable is.
        ///     For non test projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> can be used as a value for this parameter.
        ///     However, for tests projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> might be
        ///     be the folder where the test execution library is, so a different value might need to be passed.
        /// </param>
        /// <param name="loadedAssemblies"> Instance of <see cref="ILoadedAssemblies"/> used to add add all or some of currently
        ///                     loaded assemblies as dependencies for  dynamically generated assemblies.
        ///                     Use an instance of <see cref="AllLoadedAssemblies"/> to add references to all assemblies loaded into current application
        ///                     domain to the dynamically generated assembly. Use <see cref="NoLoadedAssemblies"/> to not add any additional assemblies
        ///                     references to any additional assemblies as dependencies for  dynamically generated assemblies.
        ///                     Provide your own implementation to add only some of loaded assemblies as dependencies.
        /// </param>
        public FileBasedConfigurationParameters([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider,
            [NotNull] string entryAssemblyFolder, [NotNull] ILoadedAssemblies loadedAssemblies)
        {
            ConfigurationFileContentsProvider = configurationFileContentsProvider;
            EntryAssemblyFolder = entryAssemblyFolder;
            LoadedAssemblies = loadedAssemblies;
        }

        /// <summary>
        ///     The location where the executable is.
        ///     For non test projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> can be used as a value for this parameter.
        ///     However, for tests projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> might be
        ///     be the folder where the test execution library is, so a different value might need to be passed.
        /// </summary>
        [NotNull]
        public string EntryAssemblyFolder { get; }

        /// <summary>
        /// Instance of <see cref="ILoadedAssemblies"/> used to add add all or some of currently
        /// loaded assemblies as dependencies for dynamically generated assemblies.
        /// Use an instance of <see cref="AllLoadedAssemblies"/> to add references to all assemblies loaded into current application
        /// domain to the dynamically generated assembly. Use <see cref="NoLoadedAssemblies"/> to not add any additional assemblies
        /// references to any additional assemblies as dependencies for  dynamically generated assemblies.
        /// Provide your own implementation to add only some of loaded assemblies as dependencies.
        /// </summary>
        [NotNull]
        public ILoadedAssemblies LoadedAssemblies { get; }

        /// <summary>
        /// Additional assembly file paths that will be added to dynamically generated assemblies as references.
        /// The value can be null or empty list.
        /// Example value for this property is [new string[] {@"C:\Assemblies\MyAssembly1.dll", @"C:\Assemblies\MyAssembly2.dll"}]. 
        /// Note, the assemblies in this list are added to generated assemblies only after all other references are added as part of generating the dynamic assembly,
        /// and will be added only if assemblies with similar names were not already added.
        /// For example if <see cref="AdditionalReferencedAssemblies"/> contains "C:\Assemblies\MyAssembly1.dll",
        /// and generated assembly already references "C:\Assemblies\DLLs\MyAssembly1.dll", the assembly "C:\Assemblies\MyAssembly1.dll" will not be added
        /// as a reference to the generated assembly.
        /// </summary>
        [CanBeNull, ItemNotNull]
        public IEnumerable<string> AdditionalReferencedAssemblies { get; set; }

        /// <summary>
        ///     The configuration file contents provider. An example implementation of
        ///     <see cref="IConfigurationFileContentsProvider" /> implementation is
        ///     <see cref="FileBasedConfigurationFileContentsProvider" />
        /// </summary>
        [NotNull]
        public IConfigurationFileContentsProvider ConfigurationFileContentsProvider { get; }

        /// <summary>
        /// A delegate executed when the configuration file XML document is loaded. Note, this delegate is executed after <see cref="AttributeValueTransformer"/> is used to
        /// preset some attribute values. Therefore, any changes done by <see cref="IAttributeValueTransformer"/> objects can be replaced using this delegate.
        /// </summary>
        [CanBeNull]
        public ConfigurationFileXmlDocumentLoadedEventHandler ConfigurationFileXmlDocumentLoaded { get; set; }

        /// <summary>
        /// List of attribute value providers.
        /// If the value is not null and is not an empty list, the attribute value providers in this list will be used to replace attribute values in configuration file, after the xml file is loaded, but before the loaded XML file is used to load the IoC configuration.
        /// For each instance <see cref="IAttributeValueTransformer"/> in this list the method <see cref="IAttributeValueTransformer.TryGetAttributeValue(string, System.Xml.XmlAttribute, out string)"/> will be called for each attribute to check if the value of the attribute needs to be replaced.
        /// </summary>
        [CanBeNull, ItemNotNull]
        public IEnumerable<IAttributeValueTransformer> AttributeValueTransformers { get; set; }

        /// <summary>
        /// A delegate that will be executed when dynamic assembly generation is complete.
        /// </summary>
        [CanBeNull]
        public Delegates.OnDynamicAssemblyEmitComplete OnDynamicAssemblyEmitComplete { get; set; }
    }
}