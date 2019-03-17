namespace IoC.Configuration.Tests.AutoService.Services
{
    public interface IAppInfoFactory
    {
        IAppInfo CreateAppInfo(int appId, string appDescription);
    }
}