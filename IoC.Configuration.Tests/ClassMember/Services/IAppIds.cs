namespace IoC.Configuration.Tests.ClassMember.Services
{
    public interface IAppIds
    {
        int DefaultAppId { get; }
        string DefaultAppDescription { get; }
        int GetAppId();
    }
}