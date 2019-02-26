namespace IoC.Configuration.Tests.ValueImplementation.Services
{
    public interface IAppInfo
    {
        int AppId { get; }
        string AppDescription { get; }
    }
}