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
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class Plugins : ConfigurationFileElementAbstr, IPlugins
    {
        #region Member Variables

        private readonly Dictionary<string, IPluginElement> _pluginNameToPluginMap = new Dictionary<string, IPluginElement>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region  Constructors

        public Plugins([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IPlugins Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            if (child is IPluginElement)
            {
                var plugin = (IPluginElement) child;

                if (_pluginNameToPluginMap.ContainsKey(plugin.Name))
                    throw new ConfigurationParseException(child, $"Multiple occurrences of plugin with name '{plugin.Name}'.", this);

                _pluginNameToPluginMap[plugin.Name] = plugin;
            }

            base.AddChild(child);
        }

        public IEnumerable<IPluginElement> AllPlugins => _pluginNameToPluginMap.Values;

        public IPluginElement GetPlugin(string name)
        {
            return _pluginNameToPluginMap.TryGetValue(name, out var plugin) ? plugin : null;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.PluginsDirPath))
            {
                PluginsDirectory = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.PluginsDirPath);

                //Helpers.EnsureConfigurationDirectoryExistsOrThrow(this, PluginsDirectory, ConfigurationFileAttributeNames.PluginsDirPath);
                if (!Directory.Exists(PluginsDirectory))
                    throw new ConfigurationParseException(this, $"Directory '{PluginsDirectory}' specified in attribute '{ConfigurationFileAttributeNames.PluginsDirPath}' does not exist.");
            }
            else
            {
                PluginsDirectory = string.Empty;
            }
        }

        public string PluginsDirectory { get; private set; }

        public override void ValidateOnTreeConstructed()
        {
            base.ValidateOnTreeConstructed();

            foreach (var child in Children)
            {
                if (child is IPluginElement pluginElement)
                {
                    // Lets see if there is a plugin in PluginsSetup
                    if (pluginElement.Enabled && _configuration.PluginsSetup?.GetPluginSetup(pluginElement.Name) == null)
                        throw new ConfigurationParseException(pluginElement, $"No plugin definition found under element '{ConfigurationFileElementNames.PluginsSetup}' for plugin '{pluginElement.Name}'.", this);
                }
            }
        }

        #endregion
    }
}