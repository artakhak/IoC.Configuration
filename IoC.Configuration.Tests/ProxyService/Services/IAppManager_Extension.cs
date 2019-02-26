namespace IoC.Configuration.Tests.ProxyService.Services
{
    public interface IAppManager_Extension : IAppManager, IAppManager2
    {
        IAppData DefaultApp { get; }
    }
}