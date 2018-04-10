namespace TestPluginAssembly1.Interfaces
{
    public interface IResourceAccessValidator
    {
        #region Current Type Interface

        bool ValidateAccess(object resource);

        #endregion
    }
}