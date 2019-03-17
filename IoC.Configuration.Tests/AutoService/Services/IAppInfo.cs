using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public interface IAppInfo
    {
        int AppId { get; }
        string AppDescription { get; }
    }
}
