using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class PluginSettingsElement : SettingsElement
    {
        #region  Constructors

        public PluginSettingsElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region Member Functions

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (OwningPluginElement == null)
                throw new ConfigurationParseException(this, $"The value of '{GetType().FullName}.{nameof(OwningPluginElement)}' cannot be null.");
        }

        #endregion
    }
}