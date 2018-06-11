using System.IO;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class WebApiControllerAssembly : ConfigurationFileElementAbstr, IWebApiControllerAssembly
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [CanBeNull]
        private System.Reflection.Assembly _loadedAssembly;

        #endregion

        #region  Constructors

        public WebApiControllerAssembly([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                        [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IWebApiControllerAssembly Interface Implementation

        public IAssembly Assembly { get; private set; }

        /// <summary>
        ///     Gets the loaded assembly. The value is non-null only if assembly exists and is enabled.
        ///     If assembly does not exist, an error will be log.
        ///     Assembly might be disabled if the assembly belongs to a plugin which is disabled.
        /// </summary>
        public System.Reflection.Assembly LoadedAssembly => Enabled ? _loadedAssembly : null;

        #endregion

        #region Member Functions

        public override bool Enabled => base.Enabled && Assembly != null && Assembly.Enabled;

        public override void Initialize()
        {
            base.Initialize();

            Assembly = Helpers.GetAssemblySettingByAssemblyAlias(this, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

            if (Enabled)
            {
                if (OwningPluginElement == null)
                {
                    if (Assembly.OwningPluginElement != null)
                        throw new ConfigurationParseException(this, $"Assembly  '{Assembly.Name}' belongs to plugin '{Assembly.OwningPluginElement.Name}' and can be only used in '{ElementName}' element in plugin '{Assembly.OwningPluginElement.Name}'.");
                }
                else if (OwningPluginElement != Assembly.OwningPluginElement)
                {
                    throw new ConfigurationParseException(this, $"Assembly  '{Assembly.Name}' does not belong to plugin '{OwningPluginElement.Name}'. Only assemblies of plugin  '{OwningPluginElement.Name}' can be used.");
                }

                try
                {
                    _loadedAssembly = _assemblyLocator.LoadAssembly(Path.GetFileName(Assembly.AbsolutePath), Path.GetDirectoryName(Assembly.AbsolutePath));
                }
                catch
                {
                    throw new ConfigurationParseException(this, $"Failed to load assembly '{Assembly.AbsolutePath}'.");
                }
            }
        }

        #endregion
    }
}