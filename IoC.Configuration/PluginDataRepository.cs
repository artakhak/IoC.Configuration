using System;
using System.Collections.Generic;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    public class PluginDataRepository : IPluginDataRepository
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, IPluginData> _pluginNameToPluginData = new Dictionary<string, IPluginData>(StringComparer.Ordinal);

        [NotNull]
        private readonly Dictionary<Type, IPluginData> _pluginTypeToPluginData = new Dictionary<Type, IPluginData>();

        #endregion

        #region  Constructors

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

        public IPluginData GetPluginData(string pluginName)
        {
            return _pluginNameToPluginData.TryGetValue(pluginName, out var pluginData) ? pluginData : null;
        }

        public IPluginData GetPluginData<TPluginImplementation>()
        {
            return _pluginTypeToPluginData.TryGetValue(typeof(TPluginImplementation), out var pluginData) ? pluginData : null;
        }

        public IPluginData GetPluginData(Type pluginImplementationType)
        {
            return _pluginTypeToPluginData.TryGetValue(pluginImplementationType, out var pluginData) ? pluginData : null;
        }

        public IEnumerable<IPluginData> Plugins => _pluginNameToPluginData.Values;

        #endregion
    }
}