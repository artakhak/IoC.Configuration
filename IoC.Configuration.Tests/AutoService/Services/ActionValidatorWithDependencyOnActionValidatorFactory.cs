using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public class ActionValidatorWithDependencyOnActionValidatorFactory : IActionValidator
    {
        public ActionValidatorWithDependencyOnActionValidatorFactory(IActionValidatorFactory actionValidatorFactory)
        {
            ActionValidatorFactory = actionValidatorFactory;
        }

        public bool GetIsEnabled(int actionId)
        {
            return actionId == 3;
        }

        public IActionValidatorFactory ActionValidatorFactory { get; }
    }
}
