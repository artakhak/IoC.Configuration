using SharedServices.Interfaces;

namespace TestPluginAssembly3.Implementations
{
    public class Plugin3ActionValidator : IActionValidator
    {
        public bool GetIsEnabled(int actionId)
        {
            return false;
        }
    }
}