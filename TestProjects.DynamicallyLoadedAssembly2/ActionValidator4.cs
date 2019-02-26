using SharedServices.Interfaces;
using IInterface1 = DynamicallyLoadedAssembly1.Interfaces.IInterface1;

namespace DynamicallyLoadedAssembly2
{
    public class ActionValidator4 : IActionValidator
    {
        public bool GetIsEnabled(int actionId)
        {
            return actionId != 4;
        }
    }
}