﻿// This software is part of the IoC.Configuration library
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

        public System.Reflection.Assembly LoadAssembly(string assemblyNameWithExtension, string assemblyFolder = null)
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