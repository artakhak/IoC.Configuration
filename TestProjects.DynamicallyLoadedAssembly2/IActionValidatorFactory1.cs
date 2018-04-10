using System.Collections.Generic;
using DynamicallyLoadedAssembly1.Interfaces;

namespace DynamicallyLoadedAssembly2
{
    public interface IActionValidatorFactory1
    {
        #region Current Type Interface

        IEnumerable<IActionValidator> GetInstances(int param1, string param2);

        #endregion
    }
}