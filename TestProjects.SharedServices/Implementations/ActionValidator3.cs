using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class ActionValidator3 : IActionValidator
    {
        public int IntParam { get; }

        #region  Constructors
        public ActionValidator3(int intParam)
        {
            IntParam = intParam;
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