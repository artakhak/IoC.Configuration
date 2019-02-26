using SharedServices.DataContracts;
using System;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public class StaticAndConstMembers
    {
        public static IActionValidator ActionValidator1 = new PrivateNestedActionValidator1(5);

        public readonly static Guid DefaultProjectGuid = new Guid("A7E03E0A-A1E0-4FEA-852E-37FABAAA201A");
        public const ActionTypes DefaultActionType = ActionTypes.ViewFilesList;
        public static Guid GetDefaultProjectGuid() => DefaultProjectGuid;

        private static PrivateNestedActionValidator1 _privateNestedActionValidator1 = new PrivateNestedActionValidator1(7);
        public static IActionValidator GetDefaultActionValidator() => _privateNestedActionValidator1;

        private class PrivateNestedActionValidator1 : IActionValidator
        {
            public int EnabledActionId { get; }

            public PrivateNestedActionValidator1(int enabledActionId)
            {
                EnabledActionId = enabledActionId;
            }
            public bool GetIsEnabled(int actionId)
            {
                return actionId == EnabledActionId;
            }
        }
    }
}
