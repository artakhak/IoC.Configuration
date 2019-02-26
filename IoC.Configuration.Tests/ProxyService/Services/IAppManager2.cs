using System;

namespace IoC.Configuration.Tests.ProxyService.Services
{
    public interface IAppManager2
    {
        bool IsPublicApp(Guid applicationId);
    }
}