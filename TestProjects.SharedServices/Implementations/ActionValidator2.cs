using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class ActionValidator2 : IActionValidator
    {

        #region IActionValidator Interface Implementation

        public bool GetIsEnabled(int actionId)
        {
            return actionId != 2;
        }

        #endregion

        #region Member Functions

        public int SomeDataForDiagnostics2 { get; } = 17;

        #endregion
    }
}