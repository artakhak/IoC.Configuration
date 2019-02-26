using IoC.Configuration.DiContainer;
using IoC.Configuration.Tests.ConstructedValue.Services;

namespace IoC.Configuration.Tests.ConstructedValue
{
    public class Module1 : ModuleAbstr
    {
        public Module1(IAppInfo appInfo)
        {
            AppInfo = appInfo;
        }

        public IAppInfo AppInfo { get; }
        /// <summary>
        ///     Override this method to register services. The body of overridden method might have statements like:
        ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
        ///     ().SetResolutionScope(DiResolutionScope.Singleton);
        /// </summary>
        protected override void AddServiceRegistrations()
        {
            Bind<StartupActionsRetriever>().ToSelf();
        }
    }
}
