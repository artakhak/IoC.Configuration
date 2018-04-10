using DynamicallyLoadedAssembly1.Interfaces;

namespace DynamicallyLoadedAssembly2
{
    public class ActionValidator1 : IActionValidator
    {
        #region IActionValidator Interface Implementation

        public bool GetIsEnabled(int actionId)
        {
            return actionId != 4;
        }

        #endregion

        #region Member Functions

        public int SomeDataForDiagnostics { get; } = 15;

        #endregion
    }
}