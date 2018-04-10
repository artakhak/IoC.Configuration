using System.Collections.Generic;

namespace TestPluginAssembly1.Interfaces
{
    public interface IResourceAccessValidatorFactory
    {
        #region Current Type Interface

        IEnumerable<IResourceAccessValidator> GetValidators(string resourceName);

        #endregion
    }
}