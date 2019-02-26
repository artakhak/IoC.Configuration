using SharedServices.Interfaces;
using IInterface1 = DynamicallyLoadedAssembly1.Interfaces.IInterface1;
using IInterface2 = DynamicallyLoadedAssembly1.Interfaces.IInterface2;
using IInterface3 = DynamicallyLoadedAssembly1.Interfaces.IInterface3;

namespace DynamicallyLoadedAssembly2.ClassesUsedInDeprecatedClassTests
{
    public class ActionValidator1 : IActionValidator
    {
        public ActionValidator1(IInterface1 param1, IInterface2 param2)
        {
            Property1 = param1;
            Property2 = param2;
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
        public IInterface2 Property2 { get; }

        public IInterface3 Property3 { get; set; }
        public IInterface4 Property4 { get; set; }
        #endregion
    }
}