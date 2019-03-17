namespace IoC.Configuration.Tests.ClassMember.Services
{
    public class AppInfoFactory : IAppInfoFactory
    {
        public IAppInfo CreateAppInfo(int appId, string appDescription)
        {
            return new AppInfo(appId, appDescription);
        }
    }
}