using IoC.Configuration.Tests.AutoService.Services;

namespace IoC.Configuration.Tests.ProxyService.Services
{
    public class ActionValidatorsUser
    {
        public IActionValidatorFactoryBase ActionValidatorFactory { get; }

        public ActionValidatorsUser(AutoService.Services.IActionValidatorFactoryBase actionValidatorFactory)
        {
            ActionValidatorFactory = actionValidatorFactory;
        }
    }
}
