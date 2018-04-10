using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public interface ISettingsRequestor
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<SettingInfo> RequiredSettings { get; }

        #endregion
    }
}