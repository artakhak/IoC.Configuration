using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;
using Assembly = System.Reflection.Assembly;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public class AssemblyLocator : IAssemblyLocator
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, string> _assemblyNameToPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [NotNull]
        private readonly string _dotNetAssembliesDirectory;

        [NotNull]
        private readonly string _entryAssemblyFolder;

        [NotNull]
        private readonly Func<IConfiguration> _getConfugurationFunc;

        [NotNull]
        private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region  Constructors

        public AssemblyLocator([NotNull] Func<IConfiguration> getConfugurationFunc, [NotNull] string entryAssemblyFolder)
        {
            _getConfugurationFunc = getConfugurationFunc;
            _entryAssemblyFolder = entryAssemblyFolder;
            _dotNetAssembliesDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
        }

        #endregion

        #region IAssemblyLocator Interface Implementation

        public string FindAssemblyPath(string assemblyName, string pluginName, out IList<string> searchedDirectories)
        {
            var pluginElement = pluginName == null ? null : _getConfugurationFunc?.Invoke()?.Plugins?.GetPlugin(pluginName);
            return FindAssemblyPath(assemblyName, pluginElement == null ? null : new[] {pluginElement}, null, out searchedDirectories);
        }

        public string FindAssemblyPathInAllPluginFolders(string assemblyNameWithExtension, string requestingAssemblyFolder)
        {
            return FindAssemblyPath(assemblyNameWithExtension, _getConfugurationFunc?.Invoke()?.Plugins?.AllPlugins, requestingAssemblyFolder, out var searchedDirectories);
        }

        public Assembly LoadAssembly(string assemblyNameWithExtension, string assemblyFolder)
        {
            var assemblyNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyNameWithExtension);

            if (_loadedAssemblies.TryGetValue(assemblyNameWithoutExtension, out var assembly))
                return assembly;

            foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    if (!loadedAssembly.IsDynamic && string.Compare(assemblyNameWithoutExtension, loadedAssembly.GetName().Name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        assembly = loadedAssembly;
                        break;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Warn("Failed to check an assembly.", e);
                }

            if (assembly == null)
                try
                {
                    string assemblyAbsolutePath = null;

                    if (!string.IsNullOrWhiteSpace(assemblyFolder))
                    {
                        assemblyAbsolutePath = Path.Combine(assemblyFolder, assemblyNameWithExtension);
                    }
                    else
                    {
                        assemblyAbsolutePath = FindAssemblyPathInAllPluginFolders(assemblyNameWithExtension, null);

                        if (string.IsNullOrWhiteSpace(assemblyAbsolutePath))
                            throw new Exception($"Could not find assembly '{assemblyNameWithExtension}' in probing paths and plugin directories.");
                    }

                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyAbsolutePath);
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Fatal($"Failed to load assembly '{assemblyNameWithExtension}'.", e);
                    throw;
                }

            _loadedAssemblies[assemblyNameWithoutExtension] = assembly;
            return assembly;
        }

        #endregion

        #region Member Functions

        private string FindAssemblyPath([NotNull] string assemblyName, [CanBeNull] [ItemNotNull] IEnumerable<IPluginElement> plugins, string requestingAssemblyFolder, out IList<string> searchedDirectories)
        {
            searchedDirectories = new List<string>();

            var assemblyNameWithoutExtension = assemblyName.TrimEnd().EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? Path.GetFileNameWithoutExtension(assemblyName) : assemblyName;

            if (_assemblyNameToPath.TryGetValue(assemblyNameWithoutExtension, out var assemplyPath))
            {
                searchedDirectories.Add(Path.GetDirectoryName(assemplyPath));
                return assemplyPath;
            }

            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => string.Compare(assemblyNameWithoutExtension, x.GetName().Name, StringComparison.OrdinalIgnoreCase) == 0);

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
                foreach (var plugin in plugins)
                    if (plugin.Enabled)
                        allDirectoriesToSearch.Add(plugin.GetPluginDirectory());

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