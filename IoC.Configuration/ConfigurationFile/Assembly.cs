using System;
using System.IO;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public class Assembly : ConfigurationFileElementAbstr, IAssembly
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        #endregion

        #region  Constructors

        public Assembly([NotNull] XmlElement xmlElement, IConfigurationFileElement parent, [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IAssembly Interface Implementation

        public string AbsolutePath { get; private set; }

        public string Alias { get; private set; }

        public override bool Enabled => base.Enabled && (Plugin?.Enabled ?? true);

        public override void Initialize()
        {
            base.Initialize();

            Name = this.GetNameAttributeValue();

            //if (!Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            //    throw new ConfigurationParseException(this, $"The value of '{ConfigurationFileAttributeNames.Name}' should be a file name with extension '.dll'.", false);

            if (Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                throw new ConfigurationParseException(this, $"The value of '{ConfigurationFileAttributeNames.Name}' should be an assembly file name without the file extension'.");

            Alias = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Alias);

            if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.Plugin))
            {
                var pluginName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Plugin);

                var plugin = _configuration.Plugins?.GetPlugin(pluginName);

                if (plugin == null)
                    throw new ConfigurationParseException(this, $"There is no plugin with name '{pluginName}'.");

                Plugin = plugin;
            }

            if (Enabled)
            {
                string assemblyPath = null;

                if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.OverrideDirectory))
                {
                    assemblyPath = Path.Combine(this.GetAttributeValue<string>(ConfigurationFileAttributeNames.OverrideDirectory), $"{Name}.dll");

                    if (!File.Exists(assemblyPath))
                        throw new ConfigurationParseException(this, $"Could not find assembly '{Name}'.");
                }
                else
                {
                    assemblyPath = _assemblyLocator.FindAssemblyPath(Name, Plugin?.Name, out var searchedDirectories);

                    if (string.IsNullOrWhiteSpace(assemblyPath))
                        throw new ConfigurationParseException(this, $"Could not find assembly '{Name}'. The following directories were searched: {string.Join<string>(",", from directory in searchedDirectories select "'" + directory + "'")}.");
                }

                AbsolutePath = assemblyPath;

                LogHelper.Context.Log.InfoFormat("Resolved assembly '{0}' as '{1}'", Name, assemblyPath);

                if (Plugin != null)
                {
                    var assemblyDirectory = Path.GetDirectoryName(assemblyPath);

                    if (string.Compare(assemblyDirectory, Plugin.GetPluginDirectory(), StringComparison.OrdinalIgnoreCase) != 0)
                        throw new ConfigurationParseException(this, $"The assembly '{Name}' is configured as a plugin assembly for plugin '{Plugin.Name}'. Therefore the resolved assembly file should be in plugin directory '{Plugin.GetPluginDirectory()}'. Either remove the '{ConfigurationFileAttributeNames.Plugin}' attribute, or make sure that the assembly is in plugin directory and no other assembly with the same name exists in other probing folders.");
                }

                if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.LoadAssemblyAlways) &&
                    this.GetAttributeValue<bool>(ConfigurationFileAttributeNames.LoadAssemblyAlways))
                    try
                    {
                        LogHelper.Context.Log.InfoFormat("The value of attribute '{0}' is true. Loading assembly {1}.", ConfigurationFileAttributeNames.LoadAssemblyAlways, assemblyPath);
                        _assemblyLocator.LoadAssembly(Path.GetFileName(assemblyPath), Path.GetDirectoryName(assemblyPath));
                    }
                    catch (Exception e)
                    {
                        LogHelper.Context.Log.Warn(e.Message, e);
                        throw new ConfigurationParseException(this, $"Failed to load the assembly '{assemblyPath}'.");
                    }
            }
            else
            {
                LogHelper.Context.Log.WarnFormat("Assembly '{0}' is disabled. All configuration items that use this assembly will be ignored. This, among others includes types, settings, dependency injection configurations, etc.", Name);
                AbsolutePath = Name;
            }
        }

        public string Name { get; private set; }

        public override IPluginElement OwningPluginElement => Plugin;
        public IPluginElement Plugin { get; private set; }

        #endregion

        #region Member Functions

        public override string ToString()
        {
            return $"Assembly {{{ConfigurationFileAttributeNames.Name}: '{Name}', {ConfigurationFileAttributeNames.Alias}: '{Alias}'}}";
        }

        #endregion
    }
}