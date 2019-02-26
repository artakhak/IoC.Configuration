using System;

namespace IoC.Configuration.Tests.ProxyService.Services
{
    public interface IAppData
    {
        Guid ApplicationId { get; }
        string Name { get; }
    }
}