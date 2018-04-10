using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public interface IAssemblyLocator
    {
        #region Current Type Interface

        /// <summary>
        ///     Looks for assembly in the following folders in the order specified:
        ///     - Assemblies loaded into current AppDomain,
        ///     - assemblies in .Net core default folder (Microsoft assemblies),
        ///     - assemblies in installation folder
        ///     - assemblies specified in element iocConfiguration/additionalAssemblyProbingPaths,
        ///     - assemblies in element iocConfiguration/plugins/plugin for the plugin specified in parameter
        ///     <paramref name="pluginName" />.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="pluginName">
        ///     If the value is not null, the search will be also done in plugin folder of the specified
        ///     plugin.
        /// </param>
        /// <param name="searchedDirectories">Returns the list of searched directories.</param>
        /// <returns>Returns the full path of the assembly.</returns>
        [CanBeNull]
        string FindAssemblyPath([NotNull] string assemblyName, [CanBeNull] string pluginName, [NotNull] out IList<string> searchedDirectories);

        /// <summary>
        ///     This method is similar to the other method in this class with three parameters, except it searches for assembly in
        ///     all plugin folders, and does not output
        ///     searched directories.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="requestingAssemblyFolder">If not null, the requesting assembly folder will be searched as well.</param>
        /// <returns></returns>
        [CanBeNull]
        string FindAssemblyPathInAllPluginFolders([NotNull] string assemblyName, [CanBeNull] string requestingAssemblyFolder);

        /// <summary>
        ///     Searches for the assembly in .Net directories, installation directories, probing paths and plugin folders. If the
        ///     assembly is found, it is loaded
        ///     into current application domain.
        /// </summary>
        /// <param name="assemblyNameWithExtension"></param>
        /// <param name="assemblyFolder">
        ///     If the value is not null an dis not an empty string, it will be used to generated assembly path to load.
        ///     Otherwise, assembly will be searched in multiple directories bu name.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Throws an exception if assembly fails to get loaded.</exception>
        [NotNull]
        Assembly LoadAssembly([NotNull] string assemblyNameWithExtension, [CanBeNull] string assemblyFolder = null);

        #endregion
    }
}