using System.Xml;
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class StartupActionElement : KnownServiceImplementationElement, IStartupActionElement
    {
        #region  Constructors

        public StartupActionElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent, [NotNull] IAssemblyLocator assemblyLocator)
            : base(xmlElement, parent, typeof(IStartupAction), assemblyLocator)
        {
        }

        #endregion

        #region IStartupActionElement Interface Implementation

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