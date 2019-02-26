namespace IoC.Configuration.Tests.ValueImplementation.Services
{
    public class AppInfo : IAppInfo
    {
        public AppInfo(int appId)
        {
            AppId = appId;
        }
        public AppInfo(int appId, string appDescription)
        {
            AppId = appId;
            AppDescription = appDescription;
        }

        public string AppDescription { get; set; }
        public int AppId { get; set; }
    }
}
