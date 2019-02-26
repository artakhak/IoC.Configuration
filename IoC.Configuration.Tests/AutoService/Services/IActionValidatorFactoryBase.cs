using System.Collections.Generic;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public interface IActionValidatorFactoryBase
    {
        IReadOnlyList<IActionValidator> GetValidators(int actionTypeId, string projectGuid);
    }
}
