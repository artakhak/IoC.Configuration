namespace IoC.Configuration.ConfigurationFile
{
    public class XmlElementWarning
    {
        #region  Constructors

        public XmlElementWarning(IConfigurationFileElement configurationFileElement, string warning)
        {
            ConfigurationFileElement = configurationFileElement;
            Warning = warning;
        }

        #endregion

        #region Member Functions

        public IConfigurationFileElement ConfigurationFileElement { get; }
        public string Warning { get; }

        #endregion
    }
}