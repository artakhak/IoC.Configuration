namespace TestPluginAssembly1.Interfaces
{
    public class ResourceAccessValidator2 : IResourceAccessValidator
    {
        #region IResourceAccessValidator Interface Implementation

        public bool ValidateAccess(object resource)
        {
            return true;
        }

        #endregion
    }
}