using System;

namespace IoC.Configuration.Tests.ProxyService.Services
{
    public interface IAppManager
    {
        IAppData GetApp(Guid applicationId);
    }
}