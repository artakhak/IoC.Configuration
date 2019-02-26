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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    /// <summary>
    ///     A class for searching and loading assemblies.
    /// </summary>
    /// <seealso cref="IoC.Configuration.IAssemblyLocator" />
    public class AssemblyLocator : IAssemblyLocator
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, string> _assemblyNameToPath =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [NotNull]
        private readonly string _dotNetAssembliesDirectory;

        [NotNull]
        private readonly string _entryAssemblyFolder;

        [NotNull]
        private readonly Func<IConfiguration> _getConfugurationFunc;

        [NotNull]
        private readonly Dictionary<string, System.Reflection.Assembly> _loadedAssemblies =
            new Dictionary<string, System.Reflection.Assembly>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AssemblyLocator" /> class.
        /// </summary>
        /// <param name="getConfugurationFunc">The get confuguration function.</param>
        /// <param name="entryAssemblyFolder">The entry assembly folder.</param>
        public AssemblyLocator([NotNull] Func<IConfiguration> getConfugurationFunc,
                               [NotNull] string entryAssemblyFolder)
        {
            _getConfugurationFunc = getConfugurationFunc;
            _entryAssemblyFolder = entryAssemblyFolder;
            _dotNetAssembliesDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        }

        #endregion

        #region IAssemblyLocator Interface Implementation

        /// <summary>
        ///     Searches for assembly in the following folders among others:
        ///     - Assemblies loaded into current application domain.
        ///     - assemblies in .Net core default folder (example is C:\Program
        ///     Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.0.0\ref\netcoreapp2.0).
        ///     - assemblies in executable folder.
        ///     - assemblies specified in element iocConfiguration/additionalAssemblyProbingPaths.
        ///     - assemblies specified in element iocConfiguration/plugins for the plugin specified in parameter
        ///     <paramref name="pluginName" />.
        /// </summary>
        /// <param name="assemblyName">
        ///     Assembly name. Examples are TestProjects.DynamicallyLoadedAssembly1.dll or
        ///     TestProjects.DynamicallyLoadedAssembly1
        /// </param>
        /// <param name="pluginName">
        ///     If the value is not null, the search will be also done in plugin folder of the specified plugin.
        ///     For example if the value of <paramref name="pluginName" /> is "Plugin1", and the value of attribute pluginsDirPath
        ///     in element iocConfiguration/plugins
        ///     is "c:\Plugins", then the folder c:\Plugins\Plugin1 will be searched too.
        /// </param>
        /// <param name="searchedDirectories">Returns the list of searched directories.</param>
        /// <returns>
        ///     Returns the full path of the assembly.
        /// </returns>
        public string FindAssemblyPath(string assemblyName, string pluginName, out IList<string> searchedDirectories)
        {
            var pluginElement = pluginName == null
                ? null
                : _getConfugurationFunc?.Invoke()?.Plugins?.GetPlugin(pluginName);
            return FindAssemblyPath(assemblyName, pluginElement == null ? null : new[] {pluginElement}, null,
                out searchedDirectories);
        }

        /// <summary>
        ///     This method is similar to <see cref="IAssemblyLocator.FindAssemblyPath" />() in this class, except it searches for
        ///     assembly in
        ///     all plugin folders, and does not output searched directories.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="requestingAssemblyFolder">
        ///     If not null, the folder where assembly
        ///     <paramref name="requestingAssemblyFolder" /> is, will be searched as well.
        /// </param>
        /// <returns>Returns the full path of the assembly.</returns>
        public string FindAssemblyPathInAllPluginFolders(string assemblyName,
                                                         string requestingAssemblyFolder)
        {
            return FindAssemblyPath(assemblyName, _getConfugurationFunc?.Invoke()?.Plugins?.AllPlugins,
                requestingAssemblyFolder, out var searchedDirectories);
        }

        /// <summary>
        ///     Loads the assembly into application domain, if it is not already loaded.
        ///     Looks for the assembly in number of probing directories (see comments for
        ///     <see cref="FindAssemblyPathInAllPluginFolders" />
        ///     and <see cref="IAssemblyLocator.FindAssemblyPath" /> for more details on how the assembly is searched).
        /// </summary>
        /// <param name="assemblyNameWithExtension">
        ///     Assembly file name without extension. Examples is
        ///     "TestProjects.DynamicallyLoadedAssembly1".
        /// </param>
        /// <param name="assemblyFolder">
        ///     If the value is not null and is not an empty string, the assembly will be loaded from this folder.
        ///     Otherwise, assembly will be searched in multiple directories by name.
        /// </param>
        /// <returns>
        ///     Returns the loaded assembly.
        /// </returns>
        /// <exception cref="System.Exception">
        ///     Throws an exception the assembly was not found, or if the assembly fails to get
        ///     loaded.
        /// </exception>
        public System.Reflection.Assembly LoadAssembly(string assemblyNameWithExtension, string assemblyFolder)
        {
            var assemblyNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyNameWithExtension);

            if (_loadedAssemblies.TryGetValue(assemblyNameWithoutExtension, out var assembly))
                return assembly;

            foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!loadedAssembly.IsDynamic && string.Compare(assemblyNameWithoutExtension,
                            loadedAssembly.GetName().Name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        assembly = loadedAssembly;
                        break;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Warn("Failed to check an assembly.", e);
                }
            }

            if (assembly == null)
            {
                try
                {
                    string assemblyAbsolutePath = null;

                    if (!string.IsNullOrWhiteSpace(assemblyFolder))
                        assemblyAbsolutePath = Path.Combine(assemblyFolder, assemblyNameWithExtension);
                    else
                    {
                        assemblyAbsolutePath = FindAssemblyPathInAllPluginFolders(assemblyNameWithExtension, null);

                        if (string.IsNullOrWhiteSpace(assemblyAbsolutePath))
                            throw new Exception(
                                $"Could not find assembly '{assemblyNameWithExtension}' in probing paths and plugin directories.");
                    }

                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyAbsolutePath);
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Fatal($"Failed to load assembly '{assemblyNameWithExtension}'.", e);
                    throw;
                }
            }

            _loadedAssemblies[assemblyNameWithoutExtension] = assembly;
            return assembly;
        }

        #endregion

        #region Member Functions

        private string FindAssemblyPath([NotNull] string assemblyName,
                                        [CanBeNull] [ItemNotNull] IEnumerable<IPluginElement> plugins, string requestingAssemblyFolder,
                                        out IList<string> searchedDirectories)
        {
            searchedDirectories = new List<string>();

            var assemblyNameWithoutExtension =
                assemblyName.TrimEnd().EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                    ? Path.GetFileNameWithoutExtension(assemblyName)
                    : assemblyName;

            if (_assemblyNameToPath.TryGetValue(assemblyNameWithoutExtension, out var assemplyPath))
            {
                searchedDirectories.Add(Path.GetDirectoryName(assemplyPath));
                return assemplyPath;
            }

            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x =>
                string.Compare(assemblyNameWithoutExtension, x.GetName().Name, StringComparison.OrdinalIgnoreCase) ==
                0);

            if (loadedAssembly != null)
            {
                _assemblyNameToPath[assemblyNameWithoutExtension] = loadedAssembly.Location;
                searchedDirectories.Add(Path.GetDirectoryName(loadedAssembly.Location));
                return loadedAssembly.Location;
            }

            var allDirectoriesToSearch = new List<string>();

            if (!string.IsNullOrWhiteSpace(requestingAssemblyFolder))
                allDirectoriesToSearch.Add(requestingAssemblyFolder);

            allDirectoriesToSearch.Add(_dotNetAssembliesDirectory);
            allDirectoriesToSearch.Add(_entryAssemblyFolder);

            var configuration = _getConfugurationFunc?.Invoke();
            if (configuration?.AdditionalAssemblyProbingPaths != null)
                foreach (var probingPath in configuration.AdditionalAssemblyProbingPaths.ProbingPaths)
                    if (probingPath.Enabled)
                        allDirectoriesToSearch.Add(probingPath.Path);

            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    //if (plugin.Enabled)
                    allDirectoriesToSearch.Add(plugin.GetPluginDirectory());
                }
            }

            var assemblyNameWithExtension = $"{assemblyNameWithoutExtension}.dll";
            foreach (var directory in allDirectoriesToSearch)
            {
                searchedDirectories.Add(directory);

                var assemblyFillPath = Path.Combine(directory, assemblyNameWithExtension);
                if (File.Exists(assemblyFillPath))
                {
                    _assemblyNameToPath[assemblyNameWithoutExtension] = assemblyFillPath;
                    return assemblyFillPath;
                }
            }

            return null;
        }

        #endregion
    }
}