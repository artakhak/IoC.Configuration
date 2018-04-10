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
                LogHelper.Context.Log.WarnFormat("Plugin '{0}' is disabled. All configuration items that use this plugin will be ignored. This among others included assemblies, types in assemblies, settings, dependency injection configurations, etc.", Name);
            }
        }

        public string Name { get; private set; }

        #endregion
    }
}