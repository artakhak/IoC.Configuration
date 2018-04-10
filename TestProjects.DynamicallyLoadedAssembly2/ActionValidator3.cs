using DynamicallyLoadedAssembly1.Interfaces;

namespace DynamicallyLoadedAssembly2
{
    public class ActionValidator3 : IActionValidator
    {
        #region Member Variables

        private int _constructorInjectedValue;

        #endregion

        #region  Constructors

        public ActionValidator3(int intParam)
        {
            _constructorInjectedValue = intParam;
        }

        #endregion

        #region IActionValidator Interface Implementation

        public bool GetIsEnabled(int actionId)
        {
            return actionId != 5;
        }

        #endregion

        #region Member Functions

        public int SomeDataForDiagnostics { get; } = 38;

        #endregion
    }
}