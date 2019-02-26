using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public class ActionValidator1 : IActionValidator
    {
        public ActionValidator1(IInterface1 param1)
        {
            Property1 = param1;
        }


        #region IActionValidator Interface Implementation

        public bool GetIsEnabled(int actionId)
        {
            return actionId != 4;
        }

        #endregion

        #region Member Functions

        public int SomeDataForDiagnostics { get; } = 15;
        public IInterface1 Property1 { get; }
        public IInterface2 Property2 { get; set; }
        #endregion
    }
}