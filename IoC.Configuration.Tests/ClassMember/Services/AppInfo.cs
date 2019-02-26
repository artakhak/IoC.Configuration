namespace IoC.Configuration.Tests.ClassMember.Services
{
    public class AppInfo : IAppInfo
    {
        public AppInfo()
        {
            
        }

        public AppInfo(AppTypes appType)
        {
            AppId = (int)appType;
        }

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
