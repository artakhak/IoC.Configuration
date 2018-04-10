using System.Collections.Generic;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests.TestClasses
{
    public abstract class SettingsRequestorAbstr : ISettingsRequestor
    {
        #region ISettingsRequestor Interface Implementation

        public IEnumerable<SettingInfo> RequiredSettings => new SettingInfo[0];

        #endregion
    }
}