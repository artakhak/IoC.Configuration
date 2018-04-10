using System.Collections.Generic;
using IoC.Configuration;

namespace TestPluginAssembly1.Implementations
{
    public class InvalidSettingsRequestorInPluginAssembly : ISettingsRequestor
    {
        #region ISettingsRequestor Interface Implementation

        public IEnumerable<SettingInfo> RequiredSettings => new SettingInfo[0];

        #endregion
    }
}