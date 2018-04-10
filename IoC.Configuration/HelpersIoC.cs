namespace IoC.Configuration
{
    public static class HelpersIoC
    {
        #region Member Variables

        // TODO: Add an element Version to configuration file and use to convert to latest version. 
        // Will do in next version of IoC.Configuration that will introduce changes to configuration file.
        public const string ConfigurationFileVersion = "2F7CE7FF-CB22-40B0-9691-EAC689C03A36";
        public const string IoCConfigurationSchemaName = "IoC.Configuration.Schema." + ConfigurationFileVersion + ".xsd";
        public const string OnDiContainerReadyMethodName = "OnDiContainerReady";
        public const string SchemeFileFolderRelativeLocation = "IoC.Cnfiguration.Content";

        #endregion
    }
}