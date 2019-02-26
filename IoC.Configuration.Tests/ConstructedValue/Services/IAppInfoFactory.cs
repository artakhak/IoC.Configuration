namespace IoC.Configuration.Tests.ConstructedValue.Services
{
    public interface IAppInfoFactory
    {
        IAppInfo DefaultAppInfo { get; }
        IAppInfo CreateAppInfo();
    }
}
