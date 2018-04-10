using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class PluginsSetup : ConfigurationFileElementAbstr, IPluginsSetup
    {
        #region Member Variables

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

                if (pluginSetup.Enabled)
                {
                    if (_pluginNameToPluginSetupMap.ContainsKey(pluginSetup.Plugin.Name))
                        throw new ConfigurationParseException(pluginSetup, $"Multiple occurrences of '{pluginSetup.ElementName}' for the same plugin name '{pluginSetup.Plugin.Name}'.", this);

                    _pluginNameToPluginSetupMap[pluginSetup.Plugin.Name] = pluginSetup;

                    if (_pluginTypeToPluginSetupMap.ContainsKey(pluginSetup.PluginImplementationElement.ImplementationType))
                        throw new ConfigurationParseException(pluginSetup.PluginImplementationElement, $"Multiple occurrences of '{pluginSetup.PluginImplementationElement.ElementName}' for the same plugin type '{pluginSetup.PluginImplementationElement.ImplementationType.FullName}'.", this);
                    _pluginTypeToPluginSetupMap[pluginSetup.PluginImplementationElement.ImplementationType] = pluginSetup;
                }
            }
        }

        public IEnumerable<IPluginSetup> AllPluginSetups => _pluginNameToPluginSetupMap.Values;

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