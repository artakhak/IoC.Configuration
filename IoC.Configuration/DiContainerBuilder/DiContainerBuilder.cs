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

using System;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder.CodeBased;
using IoC.Configuration.DiContainerBuilder.FileBased;
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainerBuilder
{
    /// <summary>
    ///     A DI container builder.
    /// </summary>
    public class DiContainerBuilder
    {
        #region Member Functions

        /// <summary>
        ///     Creates an instance of <see cref="ICodeBasedDiContainerConfigurator" /> for code based dependency injection
        ///     configuration.
        /// </summary>
        /// <param name="diManager">An instance of <see cref="IDiManager" /></param>
        /// <param name="entryAssemblyFolder">The entry assembly folder.</param>
        /// <param name="assemblyProbingPaths">The assembly probing paths.</param>
        /// <exception cref="ConfigurationParseException">Throws this exception if configuration parse/load fails.</exception>
        /// <exception cref="LoggerWasNotInitializedException"></exception>
        /// <exception cref="OROptimizer.DynamicCode.DynamicCodeGenerationException">Throws this exception if dynamic code generation fails.</exception>
        /// <exception cref="Exception">Throws this exception.</exception>
        public ICodeBasedDiContainerConfigurator StartCodeBasedDi([NotNull] IDiManager diManager,
                                                                  [NotNull] string entryAssemblyFolder,
                                                                  [CanBeNull] [ItemNotNull] params string[] assemblyProbingPaths)
        {
            if (!LogHelper.IsContextInitialized)
                throw new LoggerWasNotInitializedException();

            var configuration = new CodeBasedConfiguration(diManager, entryAssemblyFolder, assemblyProbingPaths);
            configuration.Init();
            return new CodeBasedDiContainerConfigurator(configuration);
        }

        /// <summary>
        ///     Creates an instance of <see cref="ICodeBasedDiContainerConfigurator" /> for code based dependency injection
        ///     configuration.
        /// </summary>
        /// <param name="diManagerClassFullName">Should be a full name of a class that implements <see cref="IDiManager" />.</param>
        /// <param name="diManagerClassAssemblyFilePath">
        ///     Full path of assembly, containing the class specified parameter
        ///     <paramref name="diManagerClassFullName" />.
        /// </param>
        /// <param name="diManagerConstructorParameters">
        ///     Collection of constructor parameter type/value combinations to be passed to a constructor in class specified
        ///     in parameter <paramref name="diManagerClassFullName" />.
        /// </param>
        /// <param name="entryAssemblyFolder">The entry assembly folder.</param>
        /// <param name="assemblyProbingPaths">Additional assembly probing paths.</param>
        /// <exception cref="ConfigurationParseException">Throws this exception if configuration parse/load fails.</exception>
        /// <exception cref="LoggerWasNotInitializedException"></exception>
        /// <exception cref="OROptimizer.DynamicCode.DynamicCodeGenerationException">Throws this exception if dynamic code generation fails.</exception>
        /// <exception cref="Exception">Throws this exception.</exception>
        public ICodeBasedDiContainerConfigurator StartCodeBasedDi([NotNull] string diManagerClassFullName,
                                                                  [NotNull] string diManagerClassAssemblyFilePath,
                                                                  [CanBeNull] [ItemNotNull] ParameterInfo[] diManagerConstructorParameters,
                                                                  [NotNull] string entryAssemblyFolder,
                                                                  [CanBeNull] [ItemNotNull] params string[] assemblyProbingPaths)
        {
            if (!LogHelper.IsContextInitialized)
                throw new LoggerWasNotInitializedException();

            var configuration = new CodeBasedConfiguration(diManagerClassFullName, diManagerClassAssemblyFilePath, diManagerConstructorParameters, entryAssemblyFolder, assemblyProbingPaths);
            configuration.Init();
            return new CodeBasedDiContainerConfigurator(configuration);
        }

        /// <summary>
        ///     Creates an instance of <see cref="IFileBasedDiContainerConfigurator" /> for file based dependency injection
        ///     configuration.
        /// </summary>
        /// <param name="configurationFileContentsProvider">
        ///     The configuration file contents provider.
        ///     An example implementation of <see cref="IConfigurationFileContentsProvider" /> implementation is
        ///     <see cref="FileBasedConfigurationFileContentsProvider" />
        /// </param>
        /// <param name="entryAssemblyFolder">The entry assembly folder.</param>
        /// <param name="configurationFileXmlDocumentLoaded">The configuration file XML document loaded.</param>
        /// <exception cref="ConfigurationParseException">Throws this exception if configuration parse/load fails.</exception>
        /// <exception cref="LoggerWasNotInitializedException"></exception>
        /// <exception cref="OROptimizer.DynamicCode.DynamicCodeGenerationException">Throws this exception if dynamic code generation fails.</exception>
        /// <exception cref="Exception">Throws this exception.</exception>
        public IFileBasedDiContainerConfigurator StartFileBasedDi([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider,
                                                                  [NotNull] string entryAssemblyFolder,
                                                                  [CanBeNull] ConfigurationFileXmlDocumentLoadedEventHandler configurationFileXmlDocumentLoaded = null)
        {
            return StartFileBasedDi(configurationFileContentsProvider, entryAssemblyFolder, out _, configurationFileXmlDocumentLoaded);
        }

        /// <summary>
        ///     Creates an instance of <see cref="IFileBasedDiContainerConfigurator" /> for file based dependency injection
        ///     configuration.
        /// </summary>
        /// <param name="configurationFileContentsProvider">
        ///     The configuration file contents provider.
        ///     An example implementation of <see cref="IConfigurationFileContentsProvider" /> implementation is
        ///     <see cref="FileBasedConfigurationFileContentsProvider" />
        /// </param>
        /// <param name="entryAssemblyFolder">The entry assembly folder.</param>
        /// <param name="loadedConfiguration">
        ///     Output parameter that returns an instance of
        ///     <see cref="ConfigurationFile.IConfiguration" />.
        /// </param>
        /// <param name="configurationFileXmlDocumentLoaded">The configuration file XML document loaded.</param>
        /// <exception cref="ConfigurationParseException">Throws this exception if configuration parse/load fails.</exception>
        /// <exception cref="LoggerWasNotInitializedException"></exception>
        /// <exception cref="OROptimizer.DynamicCode.DynamicCodeGenerationException">Throws this exception if dynamic code generation fails.</exception>
        /// <exception cref="Exception">Throws this exception.</exception>
        public IFileBasedDiContainerConfigurator StartFileBasedDi([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider,
                                                                  [NotNull] string entryAssemblyFolder,
                                                                  out IConfiguration loadedConfiguration,
                                                                  [CanBeNull] ConfigurationFileXmlDocumentLoadedEventHandler configurationFileXmlDocumentLoaded = null)
        {
            return StartFileBasedDi(configurationFileContentsProvider, entryAssemblyFolder, new AllLoadedAssemblies(), out loadedConfiguration,
                configurationFileXmlDocumentLoaded);
        }

        /// <summary>
        ///     Creates an instance of <see cref="IFileBasedDiContainerConfigurator" /> for file based dependency injection
        ///     configuration.
        /// </summary>
        /// <param name="configurationFileContentsProvider">
        ///     The configuration file contents provider.
        ///     An example implementation of <see cref="IConfigurationFileContentsProvider" /> implementation is
        ///     <see cref="FileBasedConfigurationFileContentsProvider" />
        /// </param>
        /// <param name="entryAssemblyFolder">The entry assembly folder.</param>
        /// <param name="loadedAssemblies"> Instance of <see cref="ILoadedAssemblies"/> used to add add all or some of currently
        ///                     loaded assemblies as dependencies for  dynamically generated assemblies.
        ///                     Use an instance of <see cref="AllLoadedAssemblies"/> to add references to all assemblies loaded into current application
        ///                     domain to the dynamically generated assembly. Use <see cref="NoLoadedAssemblies"/> to not add any additional assemblies
        ///                     references to any additional assemblies as dependencies for  dynamically generated assemblies.
        ///                     Provide your own implementation to add only some of loaded assemblies as dependencies.
        /// </param>
        /// <param name="loadedConfiguration">
        ///     Output parameter that returns an instance of
        ///     <see cref="ConfigurationFile.IConfiguration" />.
        /// </param>
        /// <param name="configurationFileXmlDocumentLoaded">The configuration file XML document loaded.</param>
        /// <exception cref="ConfigurationParseException">Throws this exception if configuration parse/load fails.</exception>
        /// <exception cref="LoggerWasNotInitializedException"></exception>
        /// <exception cref="OROptimizer.DynamicCode.DynamicCodeGenerationException">Throws this exception if dynamic code generation fails.</exception>
        /// <exception cref="Exception">Throws this exception.</exception>
        public IFileBasedDiContainerConfigurator StartFileBasedDi([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider,
                                                                  [NotNull] string entryAssemblyFolder,
                                                                  [NotNull] ILoadedAssemblies loadedAssemblies,  
                                                                  out IConfiguration loadedConfiguration,
                                                                  [CanBeNull] ConfigurationFileXmlDocumentLoadedEventHandler configurationFileXmlDocumentLoaded = null)
        {
            return StartFileBasedDi(new FileBasedConfigurationParameters(configurationFileContentsProvider, entryAssemblyFolder, loadedAssemblies)
            {
                ConfigurationFileXmlDocumentLoaded = configurationFileXmlDocumentLoaded
            }, out loadedConfiguration);
        }

        /// <summary>
        ///     Creates an instance of <see cref="IFileBasedDiContainerConfigurator" /> for file based dependency injection
        ///     configuration.
        /// </summary>
        /// <param name="fileBasedConfigurationParameters">An instance of <see cref="FileBasedConfigurationParameters"/> used to load and process the configuration file.</param>
        /// <param name="loadedConfiguration">
        ///     Output parameter that returns an instance of
        ///     <see cref="ConfigurationFile.IConfiguration" />.
        /// </param>
        /// <exception cref="ConfigurationParseException">Throws this exception if configuration parse/load fails.</exception>
        /// <exception cref="LoggerWasNotInitializedException"></exception>
        /// <exception cref="OROptimizer.DynamicCode.DynamicCodeGenerationException">Throws this exception if dynamic code generation fails.</exception>
        /// <exception cref="Exception">Throws this exception.</exception>
        public IFileBasedDiContainerConfigurator StartFileBasedDi([NotNull] FileBasedConfigurationParameters fileBasedConfigurationParameters,
                                                                  out IConfiguration loadedConfiguration)
        {
            if (!LogHelper.IsContextInitialized)
                throw new LoggerWasNotInitializedException();

            var configuration = new FileBasedConfiguration(fileBasedConfigurationParameters);
            configuration.Init();
            loadedConfiguration = configuration.Configuration;
            return new FileBasedDiContainerConfigurator(configuration);
        }
        #endregion
    }
}