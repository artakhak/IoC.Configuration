using IoC.Configuration.DiContainer;
using IoC.Configuration.Tests.AutoService.Services;
namespace IoC.Configuration.Tests.AutoService
{
    public class AutoServiceTestsModule : ModuleAbstr
    {
        /// <summary>
        ///     Override this method to register services. The body of overridden method might have statements like:
        ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
        ///     ().SetResolutionScope(DiResolutionScope.Singleton);
        /// </summary>
        protected override void AddServiceRegistrations()
        {
            Bind<IActionValidatorValuesProvider>().To<ActionValidatorValuesProvider>().SetResolutionScope(DiResolutionScope.Singleton);
            Bind<IInterface1>().To<Interface1_Impl1>().SetResolutionScope(DiResolutionScope.Singleton);
            Bind<IInterface2>().To<Interface2_Impl1>().SetResolutionScope(DiResolutionScope.Singleton);

            // IoC.Configuration automatically creates self-bindings for concrete classes with public constructors,
            // if one was not specified, using a DiResolutionScope.Singleton.
            // Lets explicitly provide a binding with DiResolutionScope.Transient, and test that the specified is used.
            
            Bind<ActionValidator4>().ToSelf().OnImplementationObjectActivated((container, actionValidator4) =>
                                        {
                                            actionValidator4.Property1 = 19;
                                        })
                                    .SetResolutionScope(DiResolutionScope.Transient);
        }
    }
}
