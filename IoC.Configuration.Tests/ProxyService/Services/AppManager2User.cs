namespace IoC.Configuration.Tests.ProxyService.Services
{
    public class AppManager2User
    {
        public AppManager2User(IAppManager2 appManager2)
        {
            AppManager2 = appManager2;
        }

        public IAppManager2 AppManager2 { get; }
    }
}