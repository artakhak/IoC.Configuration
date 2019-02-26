using System;
using System.Collections.Generic;
using SharedServices.DataContracts;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public interface IActionValidatorFactory : IActionValidatorFactoryBase
    {
        Guid PublicProjectId { get; }
        IActionValidator DefaultActionValidator { get; }
        IReadOnlyList<IActionValidator> GetValidators(ActionTypes actionType, Guid projectGuid);
        void SomeMethodNotInConfigFile(int param1, string param2);
        int SomePropertyNotInConfigFile { get; }
    }
}