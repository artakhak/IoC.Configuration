using IoC.Configuration.DiContainer;
using IoC.Configuration.Tests.ValueImplementation.Services;
using System.Collections.Generic;

namespace IoC.Configuration.Tests.ValueImplementation
{
    public class Module1 : ModuleAbstr
    {
        /// <summary>
        ///     Override this method to register services. The body of overridden method might have statements like:
        ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
        ///     ().SetResolutionScope(DiResolutionScope.Singleton);
        /// </summary>
        protected override void AddServiceRegistrations()
        {
            this.Bind<List<IAppInfo>>().To((diContainer) =>
                    new List<IAppInfo>
                    {
                        new AppInfo(5),
                        new AppInfo(7)
                    }
                ).SetResolutionScope(DiResolutionScope.Singleton);
        }
    }
}
