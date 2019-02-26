namespace IoC.Configuration.Tests.SettingValue.Services
{
    public class AppInfo : IAppInfo
    {
        public AppInfo(int appId)
        {
            AppId = appId;
        }

        public int AppId { get; }
        public string AppDescription { get; set; }
    }
}