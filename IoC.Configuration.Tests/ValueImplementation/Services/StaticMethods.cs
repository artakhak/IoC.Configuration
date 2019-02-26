using SharedServices.Implementations;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.ValueImplementation.Services
{
    public class StaticMethods
    {
        public const int ActionValidator3ConstructorParameter = 17;
        public static IActionValidator GetActionValidator()
        {
            return new ActionValidator3(ActionValidator3ConstructorParameter);
        }
    }
}
