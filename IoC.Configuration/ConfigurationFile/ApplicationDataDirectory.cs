using System.IO;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ApplicationDataDirectory : ConfigurationFileElementAbstr, IApplicationDataDirectory
    {
        #region  Constructors

        public ApplicationDataDirectory([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IApplicationDataDirectory Interface Implementation

        public override void Initialize()
        {
            base.Initialize();

            Path = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Path);

            if (!Directory.Exists(Path))
                throw new ConfigurationParseException(this, $"Directory '{Path}' specified in attribute '{ConfigurationFileAttributeNames.Path}' does not exist.");
        }

        public string Path { get; private set; }

        #endregion
    }
}