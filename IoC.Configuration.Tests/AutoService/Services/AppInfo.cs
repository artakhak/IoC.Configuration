namespace IoC.Configuration.Tests.AutoService.Services
{
    public class AppInfo : IAppInfo
    {
        public int AppId { get; }
        public string AppDescription { get; }

        public AppInfo(int appId, string appDescription)
        {
            AppId = appId;
            AppDescription = appDescription;
        }
    }
}