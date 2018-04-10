using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public class SettingsRequestorDefault : ISettingsRequestor
    {
        #region Member Variables

        [NotNull]
        [ItemNotNull]
        private readonly IEnumerable<SettingInfo> _requiredSettings = new List<SettingInfo>();

        #endregion

        #region ISettingsRequestor Interface Implementation

        public IEnumerable<SettingInfo> RequiredSettings => _requiredSettings;

        #endregion
    }
}