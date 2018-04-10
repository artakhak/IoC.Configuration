using System.IO;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ProbingPath : ConfigurationFileElementAbstr, IProbingPath
    {
        #region  Constructors

        public ProbingPath([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IProbingPath Interface Implementation

        public override void Initialize()
        {
            base.Initialize();

            Path = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Path);

            if (Enabled && !Directory.Exists(Path))
                throw new ConfigurationParseException(this, $"The directory specified in attribute '{ConfigurationFileAttributeNames.Path}' does not exist.");
        }

        public string Path { get; private set; }

        #endregion
    }
}