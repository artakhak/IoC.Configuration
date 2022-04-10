using SharedServices.Interfaces;

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