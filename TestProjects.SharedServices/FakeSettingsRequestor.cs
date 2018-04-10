using System.Collections.Generic;
using IoC.Configuration;
using JetBrains.Annotations;

namespace SharedServices
{
    public class FakeSettingsRequestor : ISettingsRequestor
    {
        #region Member Variables

        [NotNull]
        [ItemNotNull]
        private readonly List<SettingInfo> _requiredSettings = new List<SettingInfo>();

        #endregion

        #region  Constructors

        public FakeSettingsRequestor()
        {
            _requiredSettings.Add(new SettingInfo("MaxCharge", typeof(double)));
        }

        #endregion

        #region ISettingsRequestor Interface Implementation

        public IEnumerable<SettingInfo> RequiredSettings => _requiredSettings;

        #endregion
    }
}