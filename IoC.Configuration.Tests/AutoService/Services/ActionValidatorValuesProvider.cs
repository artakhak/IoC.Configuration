using IoC.Configuration.Tests.AutoService.Services;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public class ActionValidatorValuesProvider : IActionValidatorValuesProvider
    {
        public IActionValidator DefaultActionValidator { get; } = new PrivateActionValidator1();
        public IActionValidator AdminLevelActionValidator { get; } = new PrivateActionValidator2();

        private PrivateActionValidator2 _privateActionValidator2 = new PrivateActionValidator2();
        public IActionValidator GetViewOnlyActionvalidator() => _privateActionValidator2;

        private class PrivateActionValidator1 : IActionValidator
        {
            public bool GetIsEnabled(int actionId)
            {
                return actionId == 3;
            }
        }

        private class PrivateActionValidator2 : IActionValidator
        {
            public bool GetIsEnabled(int actionId)
            {
                return actionId == 4;
            }
        }
    }
}