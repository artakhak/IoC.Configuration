using SharedServices.DataContracts;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public class ActionValidatorDefault : IActionValidator
    {
        public bool GetIsEnabled(int actionId)
        {
            return actionId == (int)ActionTypes.ViewFilesList;
        }
    }
}