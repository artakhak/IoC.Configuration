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
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    ///     A class for searching and loading assemblies.
    /// </summary>
    public interface IAssemblyLocator
    {
        #region Current Type Interface

        /// <summary>
        ///     Searches for assembly. Also, look at the comments for this method in implementation
        ///     <seealso cref="AssemblyLocator" />.
        ///     The implementation might search for the assembly in the following locations:
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
        ///     in element iocConfiguration\plugins
        ///     is "c:\Plugins", then the folder c:\Plugins\Plugin1 will be searched too.
        /// </param>
        /// <param name="searchedDirectories">Returns the list of searched directories.</param>
        /// <returns>Returns the full path of the assembly.</returns>
        [CanBeNull]
        string FindAssemblyPath([NotNull] string assemblyName, [CanBeNull] string pluginName, [NotNull] out IList<string> searchedDirectories);

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
        [CanBeNull]
        string FindAssemblyPathInAllPluginFolders([NotNull] string assemblyName, [CanBeNull] string requestingAssemblyFolder);

        /// <summary>
        ///     Loads the assembly into application domain, if it is not already loaded.
        ///     Looks for the assembly in number of probing directories (see comments for
        ///     <see cref="IAssemblyLocator.FindAssemblyPathInAllPluginFolders" />
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
        /// <returns>Returns the loaded assembly.</returns>
        /// <exception cref="System.Exception">
        ///     Throws an exception the assembly was not found, or if the assembly fails to get
        ///     loaded.
        /// </exception>
        [NotNull]
        System.Reflection.Assembly LoadAssembly([NotNull] string assemblyNameWithExtension, [CanBeNull] string assemblyFolder = null);

        #endregion
    }
}