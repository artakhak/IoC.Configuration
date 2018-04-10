using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class SettingsRequestorImplementationElement : KnownServiceImplementationElement, ISettingsRequestorImplementationElement
    {
        #region  Constructors

        public SettingsRequestorImplementationElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent, [NotNull] IAssemblyLocator assemblyLocator)
            : base(xmlElement, parent, typeof(ISettingsRequestor), assemblyLocator)
        {
        }

        #endregion

        #region ISettingsRequestorImplementationElement Interface Implementation

        public override void Initialize()
        {
            base.Initialize();

            if (Enabled)
                if (Assembly.Plugin != null)
                    throw new ConfigurationParseException(this,
                        MessagesHelper.GetServiceImplmenentationTypeAssemblyBelongsToPluginMessage(ImplementationType, Assembly.Alias, Assembly.Plugin.Name));
        }

        #endregion
    }
}