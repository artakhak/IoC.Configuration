namespace IoC.Configuration.ConfigurationFile
{
    // TODO: Get rid off this if we are not going to use this, or use this for some other path
    public interface IApplicationDataDirectory : IConfigurationFileElement
    {
        #region Current Type Interface

        string Path { get; }

        #endregion
    }
}