using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class PluginImplementationElement : KnownServiceImplementationElement, IPluginImplementationElement
    {
        #region  Constructors

        public PluginImplementationElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent, [NotNull] IAssemblyLocator assemblyLocator)
            : base(xmlElement, parent, typeof(IPlugin), assemblyLocator)
        {
        }

        #endregion

        #region IPluginImplementationElement Interface Implementation

        public override void Initialize()
        {
            base.Initialize();

            if (Enabled)
                if (Assembly.Plugin != OwningPluginElement)
                    throw new ConfigurationParseException(this, $"The plugin implementation type '{ImplementationType.FullName}' is defined in an assembly '{Assembly.Alias}' which does not belong to plugin '{OwningPluginElement.Name}'.");
        }

        #endregion
    }
}