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
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    /// <summary>
    /// Stores plugin related data
    /// </summary>
    public class PluginData : IPluginData
    {
        #region  Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginData"/> class.
        /// </summary>
        public PluginData([NotNull] IPluginSetup pluginSetup, [NotNull] IPlugin plugin,
                          [NotNull] ISettings globalSettings,
                          [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            Settings = new PluginSettings(globalSettings, pluginSetup.SettingsElement, typeBasedSimpleSerializerAggregator);
            PluginName = pluginSetup.Plugin.Name;
            Plugin = plugin;
        }

        #endregion

        #region IPluginData Interface Implementation        
        /// <summary>
        ///  Gets the plugin defined in configuration file.
        /// </summary>
        /// <value>
        /// The plugin.
        /// </value>
        public IPlugin Plugin { get; }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        /// <value>
        /// The name of the plugin.
        /// </value>
        public string PluginName { get; }

        /// <summary>
        /// Gets the settings for the plugin that are either specified in element iocConfiguration\pluginsSetup\pluginSetup\settings or
        /// in element iocConfiguration\settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public ISettings Settings { get; }

        #endregion
    }
}