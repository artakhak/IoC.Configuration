using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Configuration.Tests.SettingValue.Services
{
    public interface IAppInfo
    {
        int AppId { get; }
        string AppDescription { get; }
    }
}
