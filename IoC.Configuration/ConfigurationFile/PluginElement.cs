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
using System.IO;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public class PluginElement : ConfigurationFileElementAbstr, IPluginElement
    {
        #region Member Variables

        [NotNull]
        private readonly IPlugins _plugins;

        #endregion

        #region  Constructors

        public PluginElement([NotNull] XmlElement xmlElement, [NotNull] IPlugins parent) : base(xmlElement, parent)
        {
            _plugins = parent;
        }

        #endregion

        #region IPluginElement Interface Implementation

        public string GetPluginDirectory()
        {
            return Path.Combine(_plugins.PluginsDirectory, Name);
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = this.GetNameAttributeValue();

            this.ValidateIdentifier(ConfigurationFileAttributeNames.Name);

            if (string.IsNullOrWhiteSpace(_plugins.PluginsDirectory))
                throw new ConfigurationParseException(this,
                    string.Format("The value of attribute '{0}' is missing. This attribute is required if there are '{1}' elements under element '{2}'.",
                        ConfigurationFileAttributeNames.PluginsDirPath, ConfigurationFileElementNames.Plugin, ConfigurationFileElementNames.Plugins), _plugins);

            var pluginDirectory = GetPluginDirectory();

            if (Enabled)
            {
                if (!Directory.Exists(pluginDirectory))
                    throw new ConfigurationParseException(this, $"Plugin directory '{pluginDirectory}' does not exist.");
            }
            else
            {
                LogHelper.Context.Log.WarnFormat("Plugin '{0}' is disabled. All configuration items that use this plugin will be ignored. This among others includes assemblies, types in assemblies, settings, dependency injection configurations, etc.", Name);
            }
        }

        public string Name { get; private set; }

        #endregion
    }
}