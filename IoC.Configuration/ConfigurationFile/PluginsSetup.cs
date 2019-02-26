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
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class PluginsSetup : ConfigurationFileElementAbstr, IPluginsSetup
    {
        #region Member Variables

        private readonly LinkedList<IPluginSetup> _allPluginSetups = new LinkedList<IPluginSetup>();

        private readonly Dictionary<string, IPluginSetup> _pluginNameToPluginSetupMap = new Dictionary<string, IPluginSetup>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Type, IPluginSetup> _pluginTypeToPluginSetupMap = new Dictionary<Type, IPluginSetup>();

        #endregion

        #region  Constructors

        public PluginsSetup([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IPluginsSetup Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IPluginSetup)
            {
                var pluginSetup = (IPluginSetup) child;

                if (_pluginNameToPluginSetupMap.ContainsKey(pluginSetup.Plugin.Name))
                    throw new ConfigurationParseException(pluginSetup, $"Multiple occurrences of '{pluginSetup.ElementName}' for the same plugin name '{pluginSetup.Plugin.Name}'.", this);

                _pluginNameToPluginSetupMap[pluginSetup.Plugin.Name] = pluginSetup;

                if (_pluginTypeToPluginSetupMap.ContainsKey(pluginSetup.PluginImplementationElement.ValueTypeInfo.Type))
                    throw new ConfigurationParseException(pluginSetup.PluginImplementationElement, $"Multiple occurrences of '{pluginSetup.PluginImplementationElement.ElementName}' for the same plugin type '{pluginSetup.PluginImplementationElement.ValueTypeInfo.TypeCSharpFullName}'.", this);
                _pluginTypeToPluginSetupMap[pluginSetup.PluginImplementationElement.ValueTypeInfo.Type] = pluginSetup;

                if (pluginSetup.Enabled)
                    _allPluginSetups.AddLast(pluginSetup);
            }
        }

        public IEnumerable<IPluginSetup> AllPluginSetups => _allPluginSetups;

        public IPluginSetup GetPluginSetup(string pluginName)
        {
            return _pluginNameToPluginSetupMap.TryGetValue(pluginName, out var pluginSetup) && pluginSetup.Enabled ? pluginSetup : null;
        }

        public IPluginSetup GetPluginSetup(Type pluginType)
        {
            return _pluginTypeToPluginSetupMap.TryGetValue(pluginType, out var pluginSetup) && pluginSetup.Enabled ? pluginSetup : null;
        }

        #endregion
    }
}