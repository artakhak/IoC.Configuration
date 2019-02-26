using SharedServices.Interfaces;

namespace TestPluginAssembly1.Implementations
{
    public class Plugin1ActionValidator : IActionValidator
    {
        public bool GetIsEnabled(int actionId)
        {
            return true;
        }
    }
}
