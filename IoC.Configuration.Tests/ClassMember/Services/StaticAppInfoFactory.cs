namespace IoC.Configuration.Tests.ClassMember.Services
{
    public static class StaticAppInfoFactory
    {
        public static IAppInfo CreateAppInfo(int appId, string appDescription)
        {
            return new AppInfo(appId, appDescription);
        }
    }
}