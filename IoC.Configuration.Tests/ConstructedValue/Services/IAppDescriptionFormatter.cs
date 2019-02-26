namespace IoC.Configuration.Tests.ConstructedValue.Services
{
    public interface IAppDescriptionFormatter
    {
        IAppInfo FormatDescription(IAppInfo appInfo);

        IAppInfo UnormatDescription(IAppInfo appInfo);
    }
}