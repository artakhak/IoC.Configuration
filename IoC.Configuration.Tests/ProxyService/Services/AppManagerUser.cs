namespace IoC.Configuration.Tests.ProxyService.Services
{
    public class AppManagerUser
    {
        public AppManagerUser(IAppManager appManager)
        {
            AppManager = appManager;
        }

        public IAppManager AppManager { get; }
    }
}
