using System.Collections.Generic;

namespace DynamicallyLoadedAssembly2.ClassesUsedInDeprecatedClassTests
{
    public interface IActionValidatorFactory1
    {
        #region Current Type Interface
        
        IEnumerable<IActionValidator> GetInstances(int param1, string param2);

        #endregion
    }
}