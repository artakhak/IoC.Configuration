using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder.CodeBased;
using IoC.Configuration.DiContainerBuilder.FileBased;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.DiContainerBuilder
{
    public class DiContainerBuilder
    {
        #region Member Functions

        public ICodeBasedDiContainerConfigurator StartCodeBasedDi([NotNull] IDiManager diManager,
                                                                  [NotNull] string entryAssemblyFolder,
                                                                  [CanBeNull] [ItemNotNull] params string[] assemblyProbingPaths)
        {
            var configuration = new CodeBasedConfiguration(diManager, entryAssemblyFolder, assemblyProbingPaths);
            configuration.Init();
            return new CodeBasedDiContainerConfigurator(configuration);
        }

        /// <summary>
        /// </summary>
        /// <param name="diManagerClassFullName">Should be full a full name of a class that implements <see cref="IDiManager" />.</param>
        /// <param name="diManagerClassAssemblyFilePath">
        ///     Full path of assembly, containing the class specified parameter
        ///     <paramref name="diManagerClassFullName" />.
        /// </param>
        /// <param name="diManagerConstructorParameters">
        ///     Collection of constructor parameter type/value combinations to be passed to a constructor in class specified
        ///     in parameter <paramref name="diManagerClassFullName" />.
        /// </param>
        /// <param name="entryAssemblyFolder"></param>
        /// <param name="assemblyProbingPaths">Additional assembly probing paths.</param>
        public ICodeBasedDiContainerConfigurator StartCodeBasedDi([NotNull] string diManagerClassFullName,
                                                                  [NotNull] string diManagerClassAssemblyFilePath,
                                                                  [CanBeNull] [ItemNotNull] ParameterInfo[] diManagerConstructorParameters,
                                                                  [NotNull] string entryAssemblyFolder,
                                                                  [CanBeNull] [ItemNotNull] params string[] assemblyProbingPaths)
        {
            var configuration = new CodeBasedConfiguration(diManagerClassFullName, diManagerClassAssemblyFilePath, diManagerConstructorParameters, entryAssemblyFolder, assemblyProbingPaths);
            configuration.Init();
            return new CodeBasedDiContainerConfigurator(configuration);
        }

        public IFileBasedDiContainerConfigurator StartFileBasedDi([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider,
                                                                  [NotNull] string entryAssemblyFolder,
                                                                  [CanBeNull] ConfigurationFileXmlDocumentLoadedEventHandler configurationFileXmlDocumentLoaded = null)
        {
            var configuration = new FileBasedConfiguration(configurationFileContentsProvider, entryAssemblyFolder, configurationFileXmlDocumentLoaded);
            configuration.Init();
            return new FileBasedDiContainerConfigurator(configuration);
        }

        #endregion
    }
}