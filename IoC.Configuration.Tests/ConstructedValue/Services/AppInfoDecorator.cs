namespace IoC.Configuration.Tests.ConstructedValue.Services
{
    public class AppInfoDecorator : IAppInfo
    {
        private readonly IAppInfo _appInfo;

        public AppInfoDecorator(IAppInfo appInfo)
        {
            _appInfo = appInfo;
            Description = $"{appInfo.Description}:{this.GetType().Name}";
        }

        public int Id => _appInfo.Id;
        public string Description { get; }
    }
}