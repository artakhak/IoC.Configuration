using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    public class PluginData : IPluginData
    {
        #region  Constructors

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

        public IPlugin Plugin { get; }

        public string PluginName { get; }

        public ISettings Settings { get; }

        #endregion
    }
}