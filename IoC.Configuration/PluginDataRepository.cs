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
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    /// <summary>
    /// A repository for data loaded from configuration file for plugins.
    /// </summary>
    public class PluginDataRepository : IPluginDataRepository
    {
        #region Member Variables
        [NotNull]
        private readonly Dictionary<string, IPluginData> _pluginNameToPluginData = new Dictionary<string, IPluginData>(StringComparer.Ordinal);


        [NotNull]
        private readonly Dictionary<Type, IPluginData> _pluginTypeToPluginData = new Dictionary<Type, IPluginData>();

        #endregion

        #region  Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginDataRepository"/> class.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public PluginDataRepository([NotNull] IPluginsSetup pluginsSetup, [NotNull] [ItemNotNull] IEnumerable<IPlugin> plugins,
                                    [NotNull] ISettings globalSettings, [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            var typeToPluginMap = new Dictionary<Type, IPlugin>();

            foreach (var plugin in plugins)
            {
                if (typeToPluginMap.ContainsKey(plugin.GetType()))
                    throw new Exception($"Multiple occurrences of plugin of type '{plugin.GetType()}'.");

                typeToPluginMap[plugin.GetType()] = plugin;
            }

            foreach (var pluginSetup in pluginsSetup.AllPluginSetups)
            {
                if (!pluginSetup.Enabled)
                    continue;

                var plugin = typeToPluginMap[pluginSetup.PluginImplementationElement.ImplementationType];
                IPluginData pluginData = new PluginData(pluginSetup,
                    plugin, globalSettings,
                    typeBasedSimpleSerializerAggregator);
                plugin.PluginData = pluginData;

                _pluginNameToPluginData[pluginSetup.Plugin.Name] = pluginData;
                _pluginTypeToPluginData[pluginSetup.PluginImplementationElement.ImplementationType] = pluginData;
            }
        }

        #endregion

        #region IPluginDataRepository Interface Implementation        
        /// <summary>
        /// Gets the plugin data for plugin with name <paramref name="pluginName" />.
        /// </summary>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <returns></returns>
        public IPluginData GetPluginData(string pluginName)
        {
            return _pluginNameToPluginData.TryGetValue(pluginName, out var pluginData) ? pluginData : null;
        }

        /// <summary>
        /// Gets the plugin data for the plugin with implementation <typeparamref name="TPluginImplementation" />. Note, the plugin implementation
        /// type can be found in element iocConfiguration/pluginsSetup/pluginSetup/pluginImplementation.
        /// </summary>
        /// <typeparam name="TPluginImplementation">The type of the plugin implementation.</typeparam>
        /// <returns></returns>
        public IPluginData GetPluginData<TPluginImplementation>()
        {
            return _pluginTypeToPluginData.TryGetValue(typeof(TPluginImplementation), out var pluginData) ? pluginData : null;
        }

        /// <summary>
        /// Gets the plugin data for the plugin with implementation specified in parameter <paramref name="pluginImplementationType" />.
        /// Note, the plugin implementation type can be found in element iocConfiguration/pluginsSetup/pluginSetup/pluginImplementation.
        /// </summary>
        /// <param name="pluginImplementationType">Type of the plugin implementation.</param>
        /// <returns></returns>
        public IPluginData GetPluginData(Type pluginImplementationType)
        {
            return _pluginTypeToPluginData.TryGetValue(pluginImplementationType, out var pluginData) ? pluginData : null;
        }

        /// <summary>
        /// Gets the plugin data objects for all enabled plugins in configuration file.
        /// </summary>
        /// <value>
        /// The plugins.
        /// </value>
        public IEnumerable<IPluginData> Plugins => _pluginNameToPluginData.Values;

        #endregion
    }
}