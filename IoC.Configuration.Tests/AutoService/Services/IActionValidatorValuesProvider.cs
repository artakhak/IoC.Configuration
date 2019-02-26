
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public interface IActionValidatorValuesProvider
    {
        IActionValidator DefaultActionValidator { get; }
        IActionValidator AdminLevelActionValidator { get; }
        IActionValidator GetViewOnlyActionvalidator();
    }
}
