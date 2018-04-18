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
using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    /// A repository for data loaded from configuration file for plugins.
    /// </summary>
    public interface IPluginDataRepository
    {
        #region Current Type Interface

        /// <summary>
        /// Gets the plugin data for plugin with name <paramref name="pluginName"/>.
        /// </summary>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <returns></returns>
        [CanBeNull]
        IPluginData GetPluginData([NotNull] string pluginName);

        /// <summary>
        /// Gets the plugin data for the plugin with implementation <typeparamref name="TPluginImplementation"/>. Note, the plugin implementation
        /// type can be found in element iocConfiguration/pluginsSetup/pluginSetup/pluginImplementation.
        /// </summary>
        /// <typeparam name="TPluginImplementation">The type of the plugin implementation.</typeparam>
        /// <returns></returns>
        [CanBeNull]
        IPluginData GetPluginData<TPluginImplementation>();

        /// <summary>
        /// Gets the plugin data for the plugin with implementation specified in parameter <paramref name="pluginImplementationType" />.
        /// Note, the plugin implementation type can be found in element iocConfiguration/pluginsSetup/pluginSetup/pluginImplementation.
        /// </summary>
        /// <param name="pluginImplementationType">Type of the plugin implementation.</param>
        /// <returns></returns>
        [CanBeNull]
        IPluginData GetPluginData(Type pluginImplementationType);

        /// <summary>
        /// Gets the plugin data objects for all enabled plugins in configuration file.
        /// </summary>
        /// <value>
        /// The plugins.
        /// </value>
        [NotNull, ItemNotNull]
        IEnumerable<IPluginData> Plugins { get; }

        #endregion
    }
}