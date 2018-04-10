using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests
{
    public class AssemblyLocatorMock : IAssemblyLocator
    {
        #region Member Variables

        private readonly IAssemblyLocator _assemblyLocator;
        private HashSet<string> _assemblyNamesWithoutExtensionToFailToLoad;
        private HashSet<string> _assemblyNamesWithoutExtensionToFailToResolve;

        #endregion

        #region  Constructors

        /// <summary>
        ///     Set of assembly names without extension to fail loading, or locate.
        /// </summary>
        /// <param name="assemblyLocator"></param>
        public AssemblyLocatorMock([NotNull] IAssemblyLocator assemblyLocator)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IAssemblyLocator Interface Implementation

        public string FindAssemblyPath(string assemblyName, string pluginName, out IList<string> searchedDirectories)
        {
            searchedDirectories = null;

            if (_assemblyNamesWithoutExtensionToFailToResolve != null && _assemblyNamesWithoutExtensionToFailToResolve.Contains(Path.GetFileNameWithoutExtension(assemblyName)))
                return null;

            return _assemblyLocator.FindAssemblyPath(assemblyName, pluginName, out searchedDirectories);
        }

        public string FindAssemblyPathInAllPluginFolders(string assemblyName, string requestingAssemblyFolder)
        {
            if (_assemblyNamesWithoutExtensionToFailToResolve != null && _assemblyNamesWithoutExtensionToFailToResolve.Contains(Path.GetFileNameWithoutExtension(assemblyName)))
                return null;

            return _assemblyLocator.FindAssemblyPathInAllPluginFolders(assemblyName, requestingAssemblyFolder);
        }

        public Assembly LoadAssembly(string assemblyNameWithExtension, string assemblyFolder = null)
        {
            if (_assemblyNamesWithoutExtensionToFailToLoad != null && _assemblyNamesWithoutExtensionToFailToLoad.Contains(Path.GetFileNameWithoutExtension(assemblyNameWithExtension)))
                throw new Exception();

            return _assemblyLocator.LoadAssembly(assemblyNameWithExtension, assemblyFolder);
        }

        #endregion

        #region Member Functions

        /// <summary>
        ///     For the specified assembles the methods <see cref="IAssemblyLocator.LoadAssembly(string, string)" /> will thrw an
        ///     exception.
        /// </summary>
        public void SetFailedToLoadAssemblies(IEnumerable<string> assemblyNamesToFailWithoutExtensions)
        {
            _assemblyNamesWithoutExtensionToFailToLoad = new HashSet<string>(assemblyNamesToFailWithoutExtensions, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     For the specified assembles the methods
        ///     <see cref="IAssemblyLocator.FindAssemblyPath(string, string, out IList{string})" />
        ///     and <see cref="IAssemblyLocator.FindAssemblyPathInAllPluginFolders(string, string)" /> will return null.
        /// </summary>
        public void SetFailedToResolveAssemblies(IEnumerable<string> assemblyNamesWithoutExtensionToFailToResolve)
        {
            _assemblyNamesWithoutExtensionToFailToResolve = new HashSet<string>(assemblyNamesWithoutExtensionToFailToResolve, StringComparer.OrdinalIgnoreCase);
        }

        #endregion
    }
}