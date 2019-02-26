namespace IoC.Configuration.Tests.ClassMember.Services
{
    public interface IAppIdToPriority
    {
        int GetPriority(int appId);
        int GetPriority(AppTypes appType);
    }
}
