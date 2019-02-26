using System.Collections.Generic;

namespace IoC.Configuration.Tests.SettingValue.Services
{
    public interface IAppIds
    {

        IReadOnlyList<int> GetAppIds(string platformType);
        int MainAppId { get; }

        int GetDefaultAppId();
    }
}
